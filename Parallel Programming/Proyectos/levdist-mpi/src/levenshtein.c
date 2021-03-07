#include <libgen.h>
#include <locale.h>
#include <omp.h>
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

// Private functions

/// Levenshtein distance algorithm compatible with Unicode
size_t levenshtein_distance_unicode(char* file1, char*file2, double* elapsed);

/// Levenshtein distance parallel ascii
size_t levenshtein_distance_ascii(char* file1, char* file2, double* elapsed, int workers);

/// Gets the data from the files with ASCII
unsigned char* levenshtein_get_data(char* path);

/// Gets the data from the files using Unicode
wchar_t* levenshtein_get_data_unicode(char *path);

/// Fill x matrix
void fill_x(size_t ** x, unsigned char* text, int workers);

/// Fill distance matrix
void fill_distance(size_t ** d, size_t ** x, unsigned char* text, unsigned char* pattern, int workers);

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

    double start_mpi = MPI_Wtime();

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

      double end_mpi = MPI_Wtime();
      *elapsed += end_mpi - start_mpi;

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

    /* Fill x matrix */
    fill_x(x, text, workers);

    /* Fill distance (window) matrix */
    fill_distance(d, x, text, pattern, workers);

    result = d[m & 1][n];

    *elapsed += walltime_elapsed(&start);

    free(text);
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

void fill_x(size_t ** x, unsigned char* text, int workers){
    size_t n = strlen((char*)text);

    #pragma omp parallel for default(none) shared(x, text, n) num_threads(workers)
    for (size_t i = 0; i < ASCII_SIZE; ++i){
        for (size_t j = 0; j < n + 1; ++j){
            if (j == 0)
                x[i][j] = 0;
            else
                if (text[j-1] == (unsigned char) i)
                    x[i][j] = j;
                else
                    x[i][j] = x[i][j - 1];
        }
    }
}

void fill_distance(size_t ** d, size_t ** x, unsigned char *text, unsigned char *pattern, int workers){
    size_t n = strlen((char*)text);
    size_t m = strlen((char*)pattern);

    size_t row = 0;
    size_t prev = 1;
    size_t temp = 0;
    size_t value_x = 0;
    size_t count = 0;

    #pragma omp parallel default(none) shared(d, x, text, pattern, m, row, prev, n) firstprivate(count) private(value_x, temp) num_threads(workers)
    {
    for (size_t i = 0; i < m + 1; ++i){
        count = 0;
        #pragma omp for
        for (size_t j = 0; j < n + 1; ++j){
            if (i == 0)
                d[row][j] = j;
            else
                if (j == 0)
                    d[row][j] = i;
                else
                    if (text[j-1] == pattern[i-1])
                        d[row][j] = d[prev][j-1];
                    else
                        if (x[pattern[i-1]][j] == 0)
                            d[row][j] = 1 + min(d[prev][j], d[prev][j-1], i + j - 1);
                        else
                            if (count != 0)
                                d[row][j] = 1 + min(d[prev][j], d[prev][j-1], d[row][j-1]);
                            else
                            {
                                value_x = x[pattern[i-1]][j];
                                temp = d[prev][value_x - 1] + (j - 1 - value_x);
                                d[row][j] = 1 + min(d[prev][j], d[prev][j-1], temp);
                            }
            ++count;
        }
        if (omp_get_thread_num() == 0){
            temp = row;
            row = prev;
            prev = temp;
        }
        #pragma omp barrier
    }
    }
}

size_t min(size_t a, size_t b, size_t c) {
    size_t m = a;
    if (b < m)
        m = b;
    if (c < m)
        m = c;
    return m;
}
