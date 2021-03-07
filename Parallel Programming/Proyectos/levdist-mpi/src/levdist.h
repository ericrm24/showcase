#ifndef LEVDIST_H
#define LEVDIST_H


/** @file levdist.h

This file contains the structures and functions that control the entire
`levdist` command. It can be thought as the global controller object
in object-oriented programming.

It may be created in the main function, for example:

```
int main(int argc, char* argv[])
{
	levdist_t levdist;
	levdist_init(&levdist);
	return levdist_run(&levdist, argc, argv);
}
```
*/

#include "arguments.h"
#include "distance.h"
#include "queue.h"


/**
	@brief Stores the atributes shared for the controller functions in this file.
*/
typedef struct
{
	/// What user asked by the command-line arguments.
	arguments_t arguments;
	/// Queue of files to be compared using Levenshtein distance.
	queue_t* files;
    /// Array of the comparisons made.
    distance_t* distances;
    /// Rank of this process for MPI
    int my_rank;
    /// Total of levdist processes running
    int process_count;
} levdist_t;


/**
	@brief Initializes a record with default information required by the controller.

	@param this Pointer to the @a levdist_t structure to be initialized.
*/
void levdist_init(levdist_t* this);


/**
	@brief Start the execution of the command

	@param this Pointer to the @a levdist_t structure with the shared attibutes.
	@param argc Argument count provided from the `main()` function.
	@param argv Argument vector provided from the `main()` function.
	@return The exit status, where 0 stands for success, otherwise for an error.
*/
int levdist_run(levdist_t* this, int argc, char* argv[]);


/**
	@brief Finds all files in the directories given by arguments, and load them to
	the @a this->files queue.

	@param this Pointer to the @a levdist_t structure with the shared attibutes.
	@param argc Argument count provided from the `main()` function.
	@param argv Argument vector provided from the `main()` function.
	@return The exit status, where 0 stands for success, otherwise for an error.
*/
int levdist_process_dirs(levdist_t* this, int argc, char* argv[]);


/**
	@brief Traverses all the directories provided by user in command-line arguments, and
	load all files to the @a this->files queue.

	@param this Pointer to the @a levdist_t structure with the shared attibutes.
	@param argc Argument count provided from the `main()` function.
	@param argv Argument vector provided from the `main()` function.
	@return The exit status, where 0 stands for success, otherwise for an error.
*/
int levdist_list_files_in_args(levdist_t* this, int argc, char* argv[]);

/**
    @brief Returns the process rank to which the current comparison belongs.

    @param index Pointer to the @a levdist_t structure with the shared attibutes.
    @param distances_size Argument count provided from the `main()` function.
    @param process_count Argument vector provided from the `main()` function.
    @return The process rank that has the result to the comparison given by index.
*/
int levdist_get_root(int index, int distances_size, int process_count);

#endif // LEVDIST_H
