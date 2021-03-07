#ifndef DISTANCE_H
#define DISTANCE_H

#include <stdlib.h>

/**
@file distance.h

Provides a structure to handle the files comparisons and subroutines such as sorting and printing.
*/

/**
 * Record containing the pair of files compared and their distance.
 */
typedef struct
{
    /// Levenshtein distance between the files
    unsigned int distance;

    /// Name of the first file
    char * file1;

    /// Name of the second file
    char * file2;
}distance_t;

/**
 * @brief Sorts an array of distances records.
 * @param distances The array to be sorted.
 * @param size The size of the array.
 */
void distance_sort (distance_t distances[], size_t size);

/**
 * @brief Prints an array of distances records.
 * @param distances The array to be printed.
 * @param size The size of the array.
 */
void distances_print (distance_t distances[], size_t size);

#endif // DISTANCE
