#include <assert.h>
#include <libgen.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <sys/stat.h>

#include "queue.h"


/// A generic node of the queue
typedef struct queue_node
{
	/// Pointer to the user data
	void* data;
	/// Next node in the queue
	struct queue_node* next;
} queue_node_t;

/// A queue of user data elements
typedef struct queue
{
	/// Number of elements in the queue
	size_t count;
	/// Pointer to the first element in the queue
	queue_node_t* head;
	/// Pointer to the last element in the queue
	queue_node_t* tail;
} queue_t;

// Private functions

// Used by qsort to sort the queue by filename
//int queue_data_compare(const void * s1, const void * s2);

void queue_quicksort(char* data[], int begin, int end);

int queue_partition(char* data[], int begin, int end);

queue_t* queue_create()
{
	// Create a record in dynamic memory with all its bits in zero
    queue_t* queue = calloc( 1, sizeof(queue_t) );
	assert(queue);
	return queue;
}

void* queue_append(queue_t* queue, void* data)
{
	assert(queue);

	// Create the node and copy the data
	queue_node_t* node = calloc(1, sizeof(queue_node_t));
	if ( node == NULL ) return NULL;
	node->data = data;

	// Append the new node to the queue
	if ( queue_is_empty(queue) )
		queue->head = queue->tail = node;
	else
		queue->tail = queue->tail->next = node;

	// Return a pointer to the data
	++queue->count;
	return node->data;
}

void* queue_peek(queue_t* queue)
{
	assert(queue);
	assert(queue_is_empty(queue) == false);

	return queue->head->data;
}

size_t queue_count(const queue_t* queue)
{
	assert(queue);

	return queue->count;
}

bool queue_is_empty(const queue_t* queue)
{
	assert(queue);

	return queue->head == NULL;
}

void queue_destroy(queue_t* queue, bool remove_data)
{
	assert(queue);

	queue_clear(queue, remove_data);
	free(queue);
}

void queue_clear(queue_t* queue, bool remove_data)
{
	assert(queue);

	while ( ! queue_is_empty(queue) )
	{
		if ( remove_data )
			free ( queue_pop(queue) );
		else
			queue_pop(queue);
	}

	queue->head = queue->tail = NULL;
}

void* queue_pop(queue_t* queue)
{
	assert(queue);
	assert(queue_is_empty(queue) == false);

	// Get a pointer to the next node and its data
	queue_node_t* node = queue->head;
	void* data = node->data;

	// Move the head to the next node
	queue->head = queue->head->next;
	--queue->count;

	// If queue bacame empty, update the tail
	if ( queue_is_empty(queue) )
		queue->tail = NULL;

	// Release the memory of the node, not its data
	free(node);
	return data;
}


queue_iterator_t queue_begin(queue_t* queue)
{
	assert(queue);
	return queue->head;
}

queue_iterator_t queue_end(queue_t* queue)
{
	(void)queue;
	return NULL;
}

queue_iterator_t queue_next(queue_iterator_t iterator)
{
	assert(iterator);
	return iterator->next;
}

void*queue_data(queue_iterator_t iterator)
{
	assert(iterator);
	return iterator->data;
}

queue_t* queue_sort(queue_t* queue)
{
    assert(queue);

    size_t size = queue_count(queue);
    char * data[size];

    int j = 0;
    for (queue_iterator_t i = queue_begin(queue), end = queue_end(queue); i != end; i = queue_next(i)) {
        data[j] = queue_data(i);
        //printf("%s\n", data[j]);
        ++j;
    }

    queue_quicksort(data, 0, size - 1);

    queue_destroy(queue, false);

    queue_t* sorted_queue = queue_create();

    for (size_t k = 0; k < size; ++k) {
        //printf("%s\n", data[k]);
        queue_append(sorted_queue, data[k]);
    }

    return sorted_queue;
}

/* To sort by filename, not path */
/*
int queue_data_compare(char* s1, char* s2)
{
    return strcmp(basename(s1), basename(s2));
}
*/

int queue_different_files(queue_t *queue)
{
    int same_file = 1;
    for (queue_iterator_t i = queue_begin(queue), end = queue_end(queue); i != end && same_file; i = queue_next(i)) {
        for (queue_iterator_t j = queue_next(i); j != end && same_file; j = queue_next(j)) {
            same_file = strcmp( queue_data(i), queue_data(j) );
        }
    }

    return same_file;
}

void queue_quicksort(char* data[], int begin, int end) {
    if ( begin < end && begin >= 0 && end >= 0 ) {
        /* Index returned by partition */
        int i = queue_partition( data, begin, end);

        queue_quicksort( data, begin, i - 1 );
        queue_quicksort( data, i + 1, end );
    }
}

int queue_partition(char* data[], int begin, int end) {
    char* pivot = data [end];
    int i = begin - 1;
    char* temp = 0;

    for ( int j = begin; j <= end - 1; ++j ) {
        if ( strcmp(data[j], pivot) <= 0 ) {
            ++i;
            temp = data[i];
            data[i] = data[j];
            data[j] = temp;
        }
    }

    temp = data[i+1];
    data[i+1] = data[end];
    data[end] = temp;

    return i + 1;
}
