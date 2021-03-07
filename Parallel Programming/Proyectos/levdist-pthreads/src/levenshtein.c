#include <libgen.h>
#include <locale.h>
#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <unistd.h>
#include <wchar.h>
#include <wctype.h>

#include "concurrency.h"
#include "levenshtein.h"

#define PATH_MAX 4096
#define ASCII_SIZE 256

/// A structure with the info each worker needs.
typedef struct
{
    pthread_t worker;
    size_t id;
    size_t worker_count;
    size_t** d;
    size_t** x;
    unsigned char* pattern;
    unsigned char* text;
    wchar_t* pattern_u;
    wchar_t* text_u;
    pthread_barrier_t* barrier;
    pthread_barrier_t* distance_barrier;
}worker_info_t;

// Private functions

/// Levenshtein distance algorithm compatible with Unicode
size_t levenshtein_distance_unicode(char* file1, char*file2, double* elapsed);

/// Levenshtein distance parallel ascii
size_t levenshtein_distance_ascii(char* file1, char* file2, double* elapsed, int workers);

/// Gets the data from the files with ASCII
unsigned char* levenshtein_get_data(char* path);

/// Gets the data from the files using Unicode
wchar_t* levenshtein_get_data_unicode(char *path);

/// Initialize workers for ASCII
void workers_init(worker_info_t* workers, size_t worker_count, size_t ** d, size_t ** x, unsigned char* pattern, unsigned char* text, pthread_barrier_t* barrier);

/// Initialize workers for Unicode
void workers_init_u(worker_info_t* workers, size_t worker_count, size_t ** d, size_t ** x, wchar_t* pattern, wchar_t* text, pthread_barrier_t* barrier);

/// Fill x matrix
void* workers_x(void* data);

/// Fill distance matrix
void* workers_distance(void* data);

size_t min(size_t a, size_t b, size_t c);


size_t levenshtein_distance(char* file1, char* file2, bool unicode, double* elapsed, int workers)
{
    if (unicode)
        return levenshtein_distance_unicode(file1, file2, elapsed);

    return levenshtein_distance_ascii(file1, file2, elapsed, workers);
}

size_t levenshtein_distance_unicode(char* file1, char*file2, double* elapsed)
{

    wchar_t* a = levenshtein_get_data_unicode(file1);
    wchar_t* b = levenshtein_get_data_unicode(file2);

    if (a == NULL || b == NULL) {
        free(a);
        free(b);
        return fprintf(stderr, "error obtaining data from files\n"),0;
    }

    walltime_t start;
    walltime_start(&start);

      unsigned int length = wcslen(a);
      unsigned int bLength = wcslen(b);
      unsigned int *cache = calloc(length, sizeof(unsigned int));
      unsigned int index = 0;
      unsigned int bIndex = 0;
      unsigned int distance;
      unsigned int bDistance;
      unsigned int result = 0;
      wchar_t code;

      /* Shortcut optimizations / degenerate cases. */
      if (a == b) {
          free(a);
          free(b);
          free(cache);
          *elapsed += walltime_elapsed(&start);
          return 0;
      }

      if (length == 0) {
          free(a);
          free(b);
          free(cache);
          *elapsed += walltime_elapsed(&start);
          return bLength;
      }

      if (bLength == 0) {
          free(a);
          free(b);
          free(cache);
          *elapsed += walltime_elapsed(&start);
          return length;
      }

      /* initialize the vector. */
      while (index < length) {
        cache[index] = index + 1;
        index++;
      }

      /* Loop. */
      while (bIndex < bLength) {
        code = b[bIndex];
        result = distance = bIndex++;
        index = -1;

        while (++index < length) {
          bDistance = code == a[index] ? distance : distance + 1;
          distance = cache[index];

          cache[index] = result = distance > result
            ? bDistance > result
              ? result + 1
              : bDistance
            : bDistance > distance
              ? distance + 1
              : bDistance;
        }
      }

      *elapsed += walltime_elapsed(&start);

      free(cache);
      free(a);
      free(b);

    return result;
}

size_t levenshtein_distance_ascii(char* file1, char* file2, double* elapsed, int workers)
{
    unsigned char* text = levenshtein_get_data(file1);
    unsigned char* pattern = levenshtein_get_data(file2);
    if (text == NULL || pattern == NULL) {
        if (text != NULL){
            free(text);
        }
        if (pattern != NULL){
            free(pattern);
        }
        return fprintf(stderr, "error obtaining data from files\n"),0;
    }

    walltime_t start;
    walltime_start(&start);

    size_t n = strlen((char*)text);
    size_t m = strlen((char*)pattern);
    size_t ** d;
    size_t ** x;
    size_t result = 0;
    worker_info_t* worker;
    pthread_barrier_t barrier;
    pthread_barrier_t distance_barrier;

    /* Shortcut optimizations / degenerate cases. */
    if (text == pattern) {
        free(text);
        free(pattern);
        *elapsed += walltime_elapsed(&start);
        return 0;
    }

    if (n == 0) {
        free(text);
        free(pattern);
        *elapsed += walltime_elapsed(&start);
        return m;
    }

    if (m == 0) {
        free(text);
        free(pattern);
        *elapsed += walltime_elapsed(&start);
        return n;
    }

    /* The largest is the text string */
    if (m > n){
        unsigned char* tmp = text;
        text = pattern;
        pattern = tmp;
        size_t temp = n;
        n = m;
        m = temp;
    }

    //printf("N: %zu\tM: %zu\nText: %sPattern: %s",n,m,text,pattern);

    /* Allocate memory */
    d = malloc(sizeof (size_t*) * 2);
    d[0] = malloc (sizeof (size_t) * (n + 1));
    d[1] = malloc (sizeof (size_t) * (n + 1));

    x = malloc(sizeof (size_t*) * ASCII_SIZE); /* ASCII */
    for (size_t index = 0; index < ASCII_SIZE; ++index){
        x[index] = malloc (sizeof (size_t) * (n + 1));
    }


    worker = malloc(sizeof (worker_info_t) * (size_t)workers);

    /* Fill x matrix */
    size_t x_workers = (size_t) workers;
    if(x_workers > ASCII_SIZE)
        x_workers = ASCII_SIZE;

    pthread_barrier_init(&barrier, NULL, (unsigned) x_workers + 1);
    workers_init(worker, x_workers, d, x, pattern, text, &barrier);

    for (size_t index = 0; index < x_workers; ++index){
        pthread_create(&worker[index].worker, NULL, workers_x, (void*) &worker[index]);
    }
    pthread_barrier_wait(&barrier);
    for (size_t index = 0; index < x_workers; ++index){
        pthread_join(worker[index].worker, NULL);
    }

    pthread_barrier_destroy(&barrier);

    /* Fill distance (window) matrix */
    size_t distance_workers = (size_t) workers;
    if (distance_workers > n + 1)
        distance_workers = n + 1;

    pthread_barrier_init(&barrier, NULL, (unsigned)distance_workers+1);
    pthread_barrier_init(&distance_barrier, NULL, (unsigned)distance_workers);

    for (size_t index = 0; index < distance_workers; ++index){
        worker[index].distance_barrier = &distance_barrier;
        pthread_create(&worker[index].worker, NULL, workers_distance, (void*) &worker[index]);
    }

    pthread_barrier_wait(&barrier);

    for (size_t index = 0; index < distance_workers; ++index){
        pthread_join(worker[index].worker, NULL);
    }

    result = d[m & 1][n];

    *elapsed += walltime_elapsed(&start);

    pthread_barrier_destroy(&barrier);
    pthread_barrier_destroy(&distance_barrier);
    free(text);
    free(worker);
    free(pattern);
    for (size_t index = 0; index < ASCII_SIZE; ++index){
        free(x[index]);
    }
    free(x);
    free(d[0]);
    free(d[1]);
    free(d);

    return result;
}

wchar_t *levenshtein_get_data_unicode(char *path)
{
    FILE* file;
    char new_path[PATH_MAX];

    /* Make sure the path is complete */
    if (strcmp(path, basename(path)) == 0) {
        sprintf(new_path, "./%s", path);
    }
    else {
        sprintf(new_path, "%s", path);
    }

    /* Get file size */
    if ( ( file = fopen( new_path, "r" ) ) == NULL ) {
        return fprintf(stderr,"Could not read file %s", path), NULL;
    }
    fseek(file, 0L, SEEK_END);
    size_t file_size = (size_t) ftell(file);
    fclose(file);


    wchar_t* data = malloc (sizeof (wchar_t) * (file_size + 1));
    wchar_t* temp = malloc (sizeof (wchar_t) * (file_size + 1));

    data[0] = L'\0';

    if ( ( file = fopen( new_path, "r" ) ) == NULL ) {
        return fprintf(stderr,"Could not read file %s", path), NULL;
    }

    while (fgetws(temp, (int)file_size, file)) {
        wcsncat(data, temp, 1000);
    }
    fclose(file);

    free (temp);

    return data;
}

unsigned char *levenshtein_get_data(char* path)
{
    FILE* file;
    char new_path[PATH_MAX];

    /* Make sure the path is complete */
    if (strcmp(path, basename(path)) == 0) {
        sprintf(new_path, "./%s", path);
    }
    else {
        sprintf(new_path, "%s", path);
    }

    /* Get file size */
    if ( ( file = fopen( new_path, "r" ) ) == NULL ) {
        return fprintf(stderr,"Could not read file %s", path), NULL;
    }
    fseek(file, 0, SEEK_END);
    size_t file_size = ftell(file);
    fseek(file, 0, SEEK_SET);

    unsigned char* data = malloc (file_size + 1);
    if (fread(data, file_size, 1, file) > file_size) {  //Not using the return value of fread was treated as an error by the compiler
        return fprintf(stderr,"An error ocurred reading file %s", path), NULL;
    }
    fclose(file);

    data[file_size] = 0;

    return data;
}

void workers_init(worker_info_t* workers, size_t worker_count, size_t **d, size_t **x, unsigned char *pattern, unsigned char *text, pthread_barrier_t* barrier)
{
    for (size_t i = 0; i < worker_count; ++i){
        workers[i].id = i;
        workers[i].worker_count = worker_count;
        workers[i].d = d;
        workers[i].x = x;
        workers[i].pattern = pattern;
        workers[i].text = text;
        workers[i].barrier = barrier;
    }
}

void workers_init_u(worker_info_t* workers, size_t worker_count, size_t **d, size_t **x, wchar_t *pattern, wchar_t *text, pthread_barrier_t* barrier)
{
    for (size_t i = 0; i < worker_count; ++i){
        workers[i].id = i;
        workers[i].worker_count = worker_count;
        workers[i].d = d;
        workers[i].x = x;
        workers[i].pattern_u = pattern;
        workers[i].text_u = text;
        workers[i].barrier = barrier;
    }
}

void* workers_x(void* data){
    worker_info_t* worker_info = (worker_info_t*) data;
    
    size_t work = (size_t) ASCII_SIZE / worker_info->worker_count;
    size_t start = work * worker_info->id;
    size_t end = work * (worker_info->id + 1);
    size_t n = strlen((char*)worker_info->text);
    if (worker_info->id == worker_info->worker_count - 1)   //Last worker
        end = ASCII_SIZE;

    for (size_t i = start; i < end; ++i){
        for (size_t j = 0; j < n + 1; ++j){
            if (j == 0){
                worker_info->x[i][j] = 0;
            }
            else{
                if(worker_info->text[j-1] == (unsigned char) i){
                    worker_info->x[i][j] = j;
                }
                else{
                    worker_info->x[i][j] = worker_info->x[i][j-1];
                }
            }
        }
    }

    pthread_barrier_wait(worker_info->barrier);

    return NULL;
}

void* workers_distance(void* data){
    worker_info_t* worker_info = (worker_info_t*) data;

    size_t n = strlen((char*)worker_info->text);
    size_t m = strlen((char*)worker_info->pattern);
    size_t row = 0;
    size_t prev = 1;
    size_t temp = 0;
    size_t value_x = 0;

    size_t work = (size_t) (n + 1) / worker_info->worker_count;
    size_t start = work * worker_info->id;
    size_t end = work * (worker_info->id + 1);
    if (worker_info->id == worker_info->worker_count - 1)   //Last worker
        end = n+1;

    for (size_t i = 0; i < m+1; ++i){
        for (size_t j = start; j < end; ++j){
            if (i == 0){
                worker_info->d[row][j] = j;
            }
            else {
                if (j == 0){
                    worker_info->d[row][j] = i;
                }
                else {
                    if (worker_info->text[j-1] == worker_info->pattern[i-1]){       // Same
                        worker_info->d[row][j] = worker_info->d[prev][j-1];
                    }
                    else {
                        if (worker_info->x[worker_info->pattern[i-1]][j] == 0){                           // Value in x = 0
                            worker_info->d[row][j] = 1 + min(worker_info->d[prev][j], worker_info->d[prev][j-1], i + j - 1);
                        }
                        else {
                            if (j != start){                                        // Levenshtein default rule
                                worker_info->d[row][j] = 1 + min(worker_info->d[prev][j], worker_info->d[prev][j-1], worker_info->d[row][j-1]);
                            }
                            else {                                                  // Special case
                                value_x = worker_info->x[worker_info->pattern[i-1]][j];
                                temp = worker_info->d[prev][value_x - 1] + (j - 1 - value_x);
                                worker_info->d[row][j] = 1 + min(worker_info->d[prev][j], worker_info->d[prev][j-1], temp);
                            }
                        }
                    }
                }
            }
        }

        temp = row;
        row = prev;
        prev = temp;

        pthread_barrier_wait(worker_info->distance_barrier);

//        if (worker_info->id == 0){
//            for (size_t j = 0; j < n+1; ++j){
//                printf("%zu ", worker_info->d[prev][j]);
//            }
//            printf("\n");
//        }
    }

    pthread_barrier_wait(worker_info->barrier);

    //printf("Thread %zu got out of iteration %zu\n", worker_info->id, worker_info->iteration);

    return NULL;
}

size_t min(size_t a, size_t b, size_t c) {
    size_t m = a;
    if (b < m)
        m = b;
    if (c < m)
        m = c;
    return m;
}
