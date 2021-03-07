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

	size_t start = 0;
	size_t end = 10;
	if (argc >=2)
	{
		if ( sscanf (argv[1], "%zd", &start) != 1 ) {
			fprintf(stderr, "error: not a valid value. Using default: 0\n");
		}
		if ( sscanf (argv[2], "%zd", &end) != 1 ) {
			fprintf(stderr, "error: not a valid value. Using default: 10\n");
		}
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

	printf("%s:%d: range [%zu, %zu[ size %zu\n", hostname, my_rank, my_start, my_end, my_end - my_start);

	size_t threads = omp_get_max_threads();

	#pragma omp parallel default (none) shared (hostname, my_rank, threads, my_start, my_end) num_threads(threads)
{
	size_t count = 0;
	size_t first = 0;
	size_t last = 0;
	size_t index = 0;
	#pragma omp for
	for (index = my_start; index < my_end; ++index){
		if (count == 0){
			first = index;
		}
		++count;
		last = index;
	}
	++last;
	printf("\t%s:%d.%d: range [%zu, %zu[ size %zu\n", hostname, my_rank, omp_get_thread_num(), first, last, count);
}

	MPI_Finalize();
	return 0;
}
