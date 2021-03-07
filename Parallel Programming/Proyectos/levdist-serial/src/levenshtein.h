#ifndef LEVENSHTEIN_H
#define LEVENSHTEIN_H

#include <stdbool.h>
#include <wchar.h>
#include <wctype.h>

/**
@file levenshtein.h

Implements the subroutines needed to calculate the Levenshtein distance.
*/

/**
@brief Compares files using the Levenshtein distance algorithm. Author of the algorithm: Titus Wormer.
@param file1 Path to the first file.
@param file2 Path to the second file.
@param unicode True if running Unicode compatibility.
@return The distance between the files.
 */
long levenshtein_distance(char *file1, char *file2 , bool unicode);

#endif // LEVENSHTEIN_H
