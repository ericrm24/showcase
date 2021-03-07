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

	int potato = 10;
	if (my_rank == 0)
	{
		if (argc ==2)
		{
			if ( sscanf (argv[1], "%d", &potato) != 1 ) {
				fprintf(stderr, "error: not a valid value. Using default: 10\n");
			}
		}
		else {
			printf("Potato initial value not given. Using default: 10\n");
		}
		--potato;
		if (potato <= 0){ /* Perdió inmediatamente */
			printf ("Process %d lost the game\n", my_rank);
			--potato;
			MPI_Send (&potato, 1, MPI_INT, next, 0, MPI_COMM_WORLD);
			MPI_Finalize();
			return 0;
		}
		MPI_Send (&potato, 1, MPI_INT, next, 0, MPI_COMM_WORLD);
	}
	while (true){
		MPI_Recv(&potato, 1, MPI_INT, MPI_ANY_SOURCE, MPI_ANY_TAG, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
		if (potato < 0) {
			--potato;
			if (potato != process_count * (-1) ) /* Último proceso, no debe enviar */
				MPI_Send (&potato, 1, MPI_INT, next, 0, MPI_COMM_WORLD);
			MPI_Finalize();
			return 0;
		}

		--potato;
		if (potato == 0){
			printf("Proccess %d lost the game\n", my_rank);
			--potato;
			MPI_Send (&potato, 1, MPI_INT, next, 0, MPI_COMM_WORLD);
			MPI_Finalize();
			return 0;
		}
		MPI_Send (&potato, 1, MPI_INT, next, 0, MPI_COMM_WORLD);

	}
	return 0;
}
