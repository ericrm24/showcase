#include <math.h>
#include <mpi.h>
#include <omp.h>
#include <stdbool.h>
#include <stdio.h>
#include <time.h>
#include <unistd.h>

/* Prime function */

bool isPrime(size_t number)
{
	if ( number < 2 ) return false;
	if ( number == 2) return true;
	if ( number % 2 == 0 ) return false;

	size_t i = 3;
	size_t last = (size_t) (double) sqrt(number);
	for ( i = 3; i <= last; i += 2 )
			if ( number % i == 0 )
					return false;

	return true;
}

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

	size_t start = 10;
	size_t end = 25;
	if (my_rank == 0)
	{
		if (argc >=2)
		{
			if ( sscanf (argv[1], "%zd", &start) != 1 ) {
				fprintf(stderr, "error: not a valid value. Using default: 0\n");
			}
			if ( sscanf (argv[2], "%zd", &end) != 1 ) {
				fprintf(stderr, "error: not a valid value. Using default: 10\n");
			}
		}
		else {
			printf("Enter the start:\n");
			if (scanf("%zd", &start) != 1 ) {
				printf("error: not a valid value. Using default: 10.\n");
			}
			printf("Enter the end:\n");
			if (scanf("%zd", &end) != 1) {
				printf("error: not a valid value. Using default: 25.\n");
			}
		}
		int dest = 0;
		for (dest = 1; dest < process_count; ++dest){
			MPI_Send (&start, 1, MPI_UNSIGNED_LONG_LONG, dest, 0, MPI_COMM_WORLD);
			MPI_Send (&end, 1, MPI_UNSIGNED_LONG_LONG, dest, 0, MPI_COMM_WORLD);
		}
	}
	else
	{
		MPI_Recv(&start, 1, MPI_UNSIGNED_LONG_LONG, 0, MPI_ANY_TAG, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
		MPI_Recv(&end, 1, MPI_UNSIGNED_LONG_LONG, 0, MPI_ANY_TAG, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
	}

	int extra = (end - start) % process_count;
	size_t work = ((end - start) / process_count);
	size_t my_start = (work * my_rank) + start;
	size_t my_end = (work * (my_rank + 1)) + start;
	if (extra > my_rank){
		my_start += my_rank;
		my_end += my_rank + 1;
	}
	else {
		my_start += extra;
		my_end += extra;
	}

	size_t threads = omp_get_max_threads();
	size_t prime_count = 0;

	double start_time, end_time;
	start_time = MPI_Wtime();

	#pragma omp parallel default (none) shared (hostname, my_rank, threads, my_start, my_end) num_threads(threads) reduction(+:prime_count)
	{
		size_t index = 0;
		#pragma omp for
		for (index = my_start; index < my_end; ++index)
		{
			if ( isPrime(index) ){
				++prime_count;
			}
		}
	}

	end_time = MPI_Wtime();
	
	printf("Process %d on %s found %zu primes in range [%zu, %zu[ in %lf with %zu threads\n", my_rank, hostname, prime_count, my_start, my_end, end_time - start_time, threads);

	MPI_Finalize();
	return 0;
}
