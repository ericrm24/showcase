#include <stdio.h>
#include <stdlib.h>
#include <locale.h>
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <wchar.h>
#include <wctype.h>

#include "distance.h"

// Private functions

/// Used to sort the distances with qsort
int distance_compare(const void *s1, const void *s2);

/// Sends to output the distance and name of the files
void distance_print (distance_t* distance);


void distance_sort(distance_t distances[], size_t size)
{
    qsort(distances, size, sizeof(distance_t), distance_compare);
}

int distance_compare(const void *s1, const void *s2)
{
    distance_t * distance1 = (distance_t *)s1;
    distance_t * distance2 = (distance_t *)s2;

    int compare = distance1->distance - distance2->distance;

    return compare;
}

void distance_print(distance_t *distance)
{
    printf("%u\t%s\t%s\n", distance->distance, distance->file1, distance->file2);
}

void distances_print(distance_t distances[], size_t size)
{
    distance_t * j = distances;
    for (size_t i = 0; i < size; ++i, ++j) {
        distance_print(j);
    }
}
