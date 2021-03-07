#include "locale.h"
#include <stdio.h>
#include <stdlib.h>
#include <mpi.h>
#include <unistd.h>

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
    MPI_Init(&argc, &argv);

    MPI_Comm_rank(MPI_COMM_WORLD, &this->my_rank);
    MPI_Comm_size(MPI_COMM_WORLD, &this->process_count);
    // Analyze the arguments given by the user
	this->arguments = arguments_analyze(argc, argv);

	// If arguments are incorrect, stop
        if ( this->arguments.error )
            return this->arguments.error;
    if (this->my_rank == 0){
        // If user asked for help or software version, print it and stop
        if ( this->arguments.help_asked )
            return arguments_print_usage();
        if ( this->arguments.version_asked )
            return arguments_print_version();

        // If user did not provided directories, stop
        if ( this->arguments.dir_count <= 0 )
            return fprintf(stderr, "levdist: error: no directories given\n"), 1;
    }
    else {
        if ( this->arguments.help_asked || this->arguments.version_asked || this->arguments.dir_count <= 0)
            return 1;
    }

	// Arguments seems fine, process the directories
	return levdist_process_dirs(this, argc, argv);
}

int levdist_process_dirs(levdist_t* this, int argc, char* argv[])
{
	// Start counting the time
	walltime_t start;
	walltime_start(&start);
    double start_mpi = MPI_Wtime();

    double comparison_elapsed = 0;
    if (this->arguments.unicode){
        setlocale(LC_ALL, "");
    }

	// Load all files into a list
	this->files = queue_create();
	levdist_list_files_in_args(this, argc, argv);

    if (this->my_rank == 0){
        if (queue_count(this->files) < 2 || ! queue_different_files(this->files)){
            queue_destroy(this->files, true);
            return fprintf(stderr, "levdist: error: at least two files are required to compare\n"), 1;
        }
    }
    else {
        if (queue_count(this->files) < 2 || ! queue_different_files(this->files)){
            queue_destroy(this->files, true);
            return 1;
        }
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

    double end_mpi = MPI_Wtime();
    double elapsed = end_mpi - start_mpi;

    double max_duration = -1.0;
    double comparison_max_duration = -1.0;
    MPI_Allreduce(&elapsed, &max_duration, 1, MPI_DOUBLE, MPI_MAX, MPI_COMM_WORLD);
    MPI_Allreduce(&comparison_elapsed, &comparison_max_duration, 1, MPI_DOUBLE, MPI_MAX, MPI_COMM_WORLD);

    if (this->my_rank == 0){
        if ( ! this->arguments.silent ) {
            // Print results
            distances_print(this->distances, distances_size);
            //levdist_print_files(this);

            if ( ! this->arguments.quiet ) {
                // Report elapsed time
                printf("Total time %.9lfs, comparing time %.9lfs, %i workers\n", elapsed, comparison_elapsed, this->arguments.workers);
            }
        }
    }
    queue_destroy(this->files, true);
    free(this->distances);

    MPI_Finalize();
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
    int my_rank, process_count = -1;
    MPI_Comm_rank(MPI_COMM_WORLD, &my_rank);
    MPI_Comm_size(MPI_COMM_WORLD, &process_count);
    size_t distances_size = (queue_count(queue) * (queue_count(queue) - 1) / 2);

    /* Distribute work */
    int work = distances_size / process_count;
    int extra = distances_size % process_count;
    int my_start = my_rank * work;
    int my_end = (my_rank + 1) * work;
    my_start += extra > my_rank ? my_rank : extra;
    my_end += extra > my_rank ? my_rank + 1 : extra;

    for (queue_iterator_t i = queue_begin(queue), end = queue_end(queue); i != end; i = queue_next(i)) {
        for (queue_iterator_t j = queue_next(i); j != end; j = queue_next(j)) {
            /* File 1 */
            distances[distances_index].file1 = queue_data(i);
            /* File 2 */
            distances[distances_index].file2 = queue_data(j);

            /* Compare */
            if (distances_index >= my_start && distances_index < my_end)
                distances[distances_index].distance = levenshtein_distance(distances[distances_index].file1, distances[distances_index].file2, unicode, elapsed, workers);
            ++distances_index;
        }
    }

    /* Share results */
    int root = 0;
    for (size_t index = 0; index < distances_size; ++index) {
      root = levdist_get_root(index, distances_size, process_count);
      MPI_Bcast(&distances[index].distance, 1, MPI_UNSIGNED_LONG_LONG, root, MPI_COMM_WORLD);
    }
}

int levdist_get_root(int index, int distances_size, int process_count)
{
    int start, end = 0;
    int work = distances_size / process_count;
    int extra = distances_size % process_count;

    for (int rank = 0; rank < process_count; ++rank){
        start = rank * work;
        end = (rank + 1) * work;
        start += extra > rank ? rank : extra;
        end += extra > rank ? rank + 1 : extra;
        if (index >= start && index < end)
            return rank;
    }
    return 0;
}
