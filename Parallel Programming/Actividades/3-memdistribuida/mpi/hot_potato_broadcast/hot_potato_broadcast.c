#include <mpi.h>
#include <omp.h>
#include <stdbool.h>
#include <stdio.h>
#include <unistd.h>

#define PASS_POTATO 0
#define OUT_OF_THE_GAME 1
#define WINNER_FOUND 2

typedef struct
{
	size_t potato;
	size_t action;
	size_t current;
	size_t in_game;
	size_t potato_init;
}potato_t;

int main(int argc, char* argv[])
{

	MPI_Init(&argc, &argv);

	int my_rank = -1;
	int process_count = -1;

	potato_t potato;

	MPI_Comm_rank(MPI_COMM_WORLD, &my_rank);
	MPI_Comm_size(MPI_COMM_WORLD, &process_count);

	bool playing = true;

	potato.potato_init = 10;
	potato.current = 0;
	potato.potato = 0;
	potato.in_game = process_count;
	potato.action = PASS_POTATO;

	if (my_rank == 0){
		if (argc >= 3){
			if ( sscanf (argv[2], "%zu", &potato.current) != 1 || potato.current >= (size_t)process_count){
				fprintf(stderr, "error: not a valid value for initial process. Using default: 0\n");
				potato.current = 0;
			}
		}
		else {
			printf("First player not given. Using default: 0\n");
			potato.current = 0;
		}
	}

	MPI_Bcast(&potato, 5, MPI_UNSIGNED_LONG_LONG, 0, MPI_COMM_WORLD);

	if ((size_t)my_rank == potato.current)
	{
		if (argc >=2)
		{
			if ( sscanf (argv[1], "%zu", &potato.potato_init) != 1 || potato.potato_init < 1 ) {
				fprintf(stderr, "error: not a valid value for potato. Using default: 10\n");
				potato.potato_init = 10;
			}
		}
		else {
			printf("Potato initial value not given. Using default: 10\n");
		}
		potato.potato = potato.potato_init;
		potato.potato = (potato.potato % 2 == 0) ? potato.potato / 2 : 3*potato.potato + 1;
		if (potato.potato == 1){ /* El siguiente pierde */
			potato.action = OUT_OF_THE_GAME;
		}
	}
	MPI_Bcast(&potato, 5, MPI_UNSIGNED_LONG_LONG, potato.current, MPI_COMM_WORLD);

	while (true){
		/* Update current player */
		potato.current = (potato.current + 1) % process_count;

		if (potato.action == WINNER_FOUND){
			MPI_Finalize();
			return 0;
		}

		if (potato.current == (size_t)my_rank && playing){
			if (potato.in_game == 1){
				printf("Process %d is the happy winner\n", my_rank);
				fflush(stdout);
				potato.action = WINNER_FOUND;
			}
			else {
				if (potato.action == OUT_OF_THE_GAME){
					playing = false;
					potato.potato = potato.potato_init;
					--potato.in_game;
					potato.action = PASS_POTATO;
				}
				else {				/* PASS_POTATO */
					potato.potato = (potato.potato % 2 == 0) ? potato.potato / 2 : 3*potato.potato + 1;
					if (potato.potato == 1)
						potato.action = OUT_OF_THE_GAME;
				}
			}
		}

		MPI_Bcast(&potato, 5, MPI_UNSIGNED_LONG_LONG, potato.current, MPI_COMM_WORLD);

	}
	return 0;
}
