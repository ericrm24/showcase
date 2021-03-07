#include <libgen.h>
#include <locale.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <wchar.h>
#include <wctype.h>

#include "levenshtein.h"

#define PATH_MAX 4096

// Private functions

/// Levenshtein distance algorithm compatible with Unicode
unsigned int levenshtein_distance_unicode(char* file1, char*file2);

/// Levenshtein distance algorithm with ASCII
unsigned int levenshtein_distance_ascii(char* file1, char*file2);

/// Gets the data from the files with ASCII
char* levenshtein_get_data(char* path);

/// Gets the data from the files using Unicode
wchar_t* levenshtein_get_data_unicode(char *path);

/// Returns the minimum
int min(int i, int j);


long levenshtein_distance(char* file1, char* file2, bool unicode) {
    if (unicode)
        return levenshtein_distance_unicode(file1, file2);

    return levenshtein_distance_ascii(file1, file2);
}

unsigned int levenshtein_distance_unicode (char *file1, char *file2)
{
    setlocale(LC_ALL, "");

    wchar_t* a = levenshtein_get_data_unicode(file1);
    wchar_t* b = levenshtein_get_data_unicode(file2);

    if (a == NULL || b == NULL) {
        if (a != NULL)
            free(a);
        if (b != NULL)
            free(b);
        return fprintf(stderr, "error obtaining data from files\n"),0;
    }

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
          return 0;
      }

      if (length == 0) {
          free(a);
          free(b);
          free(cache);
          return bLength;
      }

      if (bLength == 0) {
          free(a);
          free(b);
          free(cache);
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

      free(cache);
      free(a);
      free(b);

    return result;
}

unsigned int levenshtein_distance_ascii (char *file1, char *file2)
{
    setlocale(LC_ALL, "");

    char* a = levenshtein_get_data(file1);
    char* b = levenshtein_get_data(file2);
    if (a == NULL || b == NULL) {
        if (a != NULL)
            free(a);
        if (b != NULL)
            free(b);
        return fprintf(stderr, "error obtaining data from files\n"),0;
    }

      unsigned int length = strlen(a);
      unsigned int bLength = strlen(b);
      unsigned int *cache = calloc(length, sizeof(unsigned int));
      unsigned int index = 0;
      unsigned int bIndex = 0;
      unsigned int distance;
      unsigned int bDistance;
      unsigned int result;
      char code;

      /* Shortcut optimizations / degenerate cases. */
      if (a == b) {
          free(a);
          free(b);
          free(cache);
          return 0;
      }

      if (length == 0) {
          free(a);
          free(b);
          free(cache);
          return bLength;
      }

      if (bLength == 0) {
          free(a);
          free(b);
          free(cache);
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

      free(cache);
      free(a);
      free(b);

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
    size_t file_size = ftell(file);
    fclose(file);


    wchar_t* data = malloc (sizeof (wchar_t) * (file_size + 1));
    wchar_t* temp = malloc (sizeof (wchar_t) * (file_size + 1));

    data[0] = L'\0';

    if ( ( file = fopen( new_path, "r" ) ) == NULL ) {
        return fprintf(stderr,"Could not read file %s", path), NULL;
    }

    while (fgetws(temp, file_size, file)) {
        wcsncat(data, temp, 1000);
    }
    fclose(file);

    free (temp);

    return data;
}

char* levenshtein_get_data(char* path) {
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

    char* data = malloc (file_size + 1);
    if (fread(data, file_size, 1, file) > file_size) {  //Not using the return value of fread was treated as an error by the compiler
        return fprintf(stderr,"An error ocurred reading file %s", path), NULL;
    }
    fclose(file);

    data[file_size] = 0;

    return data;
}
