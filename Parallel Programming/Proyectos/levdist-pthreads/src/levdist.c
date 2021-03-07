#include "locale.h"
#include <stdio.h>
#include <stdlib.h>

#include "concurrency.h"
#include "dir.h"
#include "levdist.h"
#include "levenshtein.h"

// Private functions:

/// Shows how to travese the list removing elements
void levdist_print_and_destroy_files(levdist_t* this);

/// Shows how to traverse the list without removing elements
void levdist_print_files(levdist_t* this);

/// Processes the queue to calculate the distances
void levdist_process_queue (distance_t distances[], queue_t* queue, bool unicode, double* elapsed, int workers);


void levdist_init(levdist_t* this)
{
	arguments_init(&this->arguments);
	this->files = NULL;
    this->distances = NULL;
}

int levdist_run(levdist_t* this, int argc, char* argv[])
{
    // Analyze the arguments given by the user
	this->arguments = arguments_analyze(argc, argv);

	// If arguments are incorrect, stop
	if ( this->arguments.error )
		return this->arguments.error;

	// If user asked for help or software version, print it and stop
	if ( this->arguments.help_asked )
		return arguments_print_usage();
	if ( this->arguments.version_asked )
		return arguments_print_version();

	// If user did not provided directories, stop
	if ( this->arguments.dir_count <= 0 )
		return fprintf(stderr, "levdist: error: no directories given\n"), 1;

	// Arguments seems fine, process the directories
	return levdist_process_dirs(this, argc, argv);
}

int levdist_process_dirs(levdist_t* this, int argc, char* argv[])
{
	// Start counting the time
	walltime_t start;
	walltime_start(&start);

    double comparison_elapsed = 0;
    if (this->arguments.unicode){
        setlocale(LC_ALL, "");
    }

	// Load all files into a list
	this->files = queue_create();
	levdist_list_files_in_args(this, argc, argv);

    if (queue_count(this->files) < 2 || ! queue_different_files(this->files)){
        queue_destroy(this->files, true);
        return fprintf(stderr, "levdist: error: at least two files are required to compare\n"), 1;
    }

    // Create distances array
    size_t queue_size = queue_count(this->files);
    size_t distances_size = (queue_size * (queue_size - 1) / 2);
    this->distances = malloc(sizeof (distance_t) * distances_size);

    //Sort queue
    this->files = queue_sort(this->files);

    // Process files queue
    levdist_process_queue(this->distances, this->files, this->arguments.unicode, &comparison_elapsed, this->arguments.workers);

    distance_sort(this->distances, distances_size);

    if ( ! this->arguments.silent ) {
        // Print results
        distances_print(this->distances, distances_size);
        //levdist_print_files(this);

        if ( ! this->arguments.quiet ) {
            // Report elapsed time
            printf("Total time %.9lfs, comparing time %.9lfs, %i workers\n", walltime_elapsed(&start), comparison_elapsed, this->arguments.workers);
        }
    }
    queue_destroy(this->files, true);
    free(this->distances);

	return 0;
}

int levdist_list_files_in_args(levdist_t* this, int argc, char* argv[])
{
	// Traverse all arguments
	for ( int current = 1; current < argc; ++current )
	{
		// Skip command-line options
		const char* arg = argv[current];
		if ( *arg == '-' )
			continue;

		dir_list_files_in_dir(this->files, arg, this->arguments.recursive);
	}

	return 0;
}

void levdist_print_and_destroy_files(levdist_t* this)
{
	long count = 0;
	while ( ! queue_is_empty(this->files) )
	{
		char* filename = (char*)queue_pop(this->files);
		printf("%ld: %s\n", ++count, filename);
		free(filename);
	}
}

void levdist_print_files(levdist_t* this)
{
	long count = 0;
	for ( queue_iterator_t itr = queue_begin(this->files); itr != queue_end(this->files); itr = queue_next(itr) )
	{
		const char* filename = (const char*)queue_data(itr);
		printf("%ld: %s\n", ++count, filename);
	}
}

void levdist_process_queue(distance_t* distances, queue_t *queue, bool unicode, double* elapsed, int workers)
{
    int distances_index = 0;
    for (queue_iterator_t i = queue_begin(queue), end = queue_end(queue); i != end; i = queue_next(i)) {
        for (queue_iterator_t j = queue_next(i); j != end; j = queue_next(j)) {
            /* File 1 */
            distances[distances_index].file1 = queue_data(i);
            /* File 2 */
            distances[distances_index].file2 = queue_data(j);

            /* Compare */
            distances[distances_index].distance = levenshtein_distance(distances[distances_index].file1, distances[distances_index].file2, unicode, elapsed, workers);
            ++distances_index;
        }
    }
}
