#include <assert.h>
#include <stdlib.h>

#include "array.h"

array_t array_create(size_t capacity)
{
	assert(capacity);
    array_t array;
    array.capacity = capacity;
    array.count = 0;
    pthread_rwlock_init(&array.rwlock, NULL);
    array.elements = malloc( capacity * sizeof(void*) );
    return array;
}

void array_destroy(array_t *array)
{
    pthread_rwlock_destroy(&array->rwlock);
    free(array->elements);
}

int array_increase_capacity(array_t *array)
{
    pthread_rwlock_wrlock(&array->rwlock);

    size_t new_capacity = 10 * array->capacity;
    void** new_elements = (void**)realloc( array->elements, new_capacity * sizeof(void*) );
	if ( new_elements == NULL )
		return -1;

    array->capacity = new_capacity;
    array->elements = new_elements;

    pthread_rwlock_unlock(&array->rwlock);

	return 0; // Success
}

int array_decrease_capacity(array_t *array)
{
    pthread_rwlock_wrlock(&array->rwlock);

    size_t new_capacity = array->capacity / 10;
	if ( new_capacity < 10 )
		return 0;

    void** new_elements = (void**)realloc( array->elements, new_capacity * sizeof(void*) );
	if ( new_elements == NULL )
		return -1;

    array->capacity = new_capacity;
    array->elements = new_elements;

    pthread_rwlock_unlock(&array->rwlock);

	return 0; // Success
}

size_t array_get_count(array_t *array)
{
    pthread_rwlock_rdlock(&array->rwlock);

    assert(array);
    size_t count = array->count;

    pthread_rwlock_unlock(&array->rwlock);
    return count;
}

void* array_get_element(array_t *array, size_t index)
{
	assert( index < array_get_count(array) );

    pthread_rwlock_rdlock(&array->rwlock);

    void* element = array->elements[index];

    pthread_rwlock_unlock(&array->rwlock);

    return element;
}

int array_append(array_t *array, void* element)
{
    if ( array->count == array->capacity )
		if ( ! array_increase_capacity(array) )
			return -1;

    pthread_rwlock_wrlock(&array->rwlock);

    array->elements[array->count++] = element;

    pthread_rwlock_unlock(&array->rwlock);

	return 0; // Success
}

size_t array_find_first(array_t *array, void *element, size_t start_pos)
{
    pthread_rwlock_rdlock(&array->rwlock);

    for ( size_t index = start_pos; index < array->count; ++index ) {
        if ( array->elements[index] == element ){
            pthread_rwlock_unlock(&array->rwlock);
            return index;
        }
    }

    pthread_rwlock_unlock(&array->rwlock);
	return array_not_found;
}

int array_remove_first(array_t *array, void *element, size_t start_pos)
{
    pthread_rwlock_wrlock(&array->rwlock);

    size_t index = array_find_first(array, element, start_pos);
    if ( index == array_not_found ){
        pthread_rwlock_unlock(&array->rwlock);
        return -1;
    }


    for ( --array->count; index < array->count; ++index ){
        array->elements[index] = array->elements[index + 1];
    }
    if ( array->count == array->capacity / 10 )
		array_decrease_capacity(array);

    pthread_rwlock_unlock(&array->rwlock);
    return 0; // Removed
}
