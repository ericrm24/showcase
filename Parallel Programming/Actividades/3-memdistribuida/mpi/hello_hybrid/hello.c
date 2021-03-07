#include <mpi.h>
#include <omp.h>
#include <stdio.h>

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

	printf("Hello from process %d of %d on %s\n", my_rank, process_count, hostname);

	size_t threads = omp_get_max_threads();
	size_t index = 0;

	#pragma omp parallel for default (none) shared (hostname, my_rank, threads) num_threads(threads)
	for (index = 0; index < threads; ++index){
		printf("\tHello from thread %d of %d of process %d on %s\n", omp_get_thread_num(), omp_get_num_threads(), my_rank, hostname);
	}

	MPI_Finalize();
	return 0;
}
