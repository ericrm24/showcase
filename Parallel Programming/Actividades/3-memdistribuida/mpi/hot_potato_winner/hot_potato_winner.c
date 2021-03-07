#include <mpi.h>
#include <omp.h>
#include <stdbool.h>
#include <stdio.h>
#include <unistd.h>

int main(int argc, char* argv[])
{

	MPI_Init(&argc, &argv);

	int my_rank = -1;
	int process_count = -1;

	MPI_Comm_rank(MPI_COMM_WORLD, &my_rank);
	MPI_Comm_size(MPI_COMM_WORLD, &process_count);

	char hostname[MPI_MAX_PROCESSOR_NAME];
	int len_hostname = -1;
	MPI_Get_processor_name(hostname, &len_hostname);
	int next = (my_rank+1)%process_count;
	size_t finished = 0;
	bool playing = true;

	int potato = 0;
	int potato_init = 10;
	if (my_rank == 0)
	{
		if (argc ==2)
		{
			if ( sscanf (argv[1], "%d", &potato_init) != 1 || potato_init < 0 ) {
				fprintf(stderr, "error: not a valid value. Using default: 10\n");
				potato_init = 10;
			}
		}
		else {
			printf("Potato initial value not given. Using default: 10\n");
		}
		potato = potato_init;
		--potato;
		if (potato <= 0){ /* Perdió inmediatamente */
			playing = false;
			++finished;
			potato = potato_init;
		}
		MPI_Send (&potato, 1, MPI_INT, next, 0, MPI_COMM_WORLD);
		MPI_Send (&finished, 1, MPI_UNSIGNED_LONG_LONG, next, 0, MPI_COMM_WORLD);
	}
	MPI_Bcast(&potato_init, 1, MPI_INT, 0, MPI_COMM_WORLD);

	while (true){
		MPI_Recv(&potato, 1, MPI_INT, MPI_ANY_SOURCE, MPI_ANY_TAG, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
		MPI_Recv(&finished, 1, MPI_UNSIGNED_LONG_LONG, MPI_ANY_SOURCE, MPI_ANY_TAG, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
		if (finished == (size_t) process_count - 1){
			if (playing){
				printf("Process %d is the happy winner\n", my_rank);
				fflush(stdout);
			}
			MPI_Send (&potato, 1, MPI_INT, next, 0, MPI_COMM_WORLD);
			MPI_Send (&finished, 1, MPI_UNSIGNED_LONG_LONG, next, 0, MPI_COMM_WORLD);
			return 0;
		}
		else {
			if (playing)
				--potato;
			if (potato == 0){
				playing = false;
				potato = potato_init;
				++finished;
			}
		}

		MPI_Send (&potato, 1, MPI_INT, next, 0, MPI_COMM_WORLD);
		MPI_Send (&finished, 1, MPI_UNSIGNED_LONG_LONG, next, 0, MPI_COMM_WORLD);

	}
	return 0;
}
