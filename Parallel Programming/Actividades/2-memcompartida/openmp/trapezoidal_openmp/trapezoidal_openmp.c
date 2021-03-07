#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <omp.h>
#include <time.h>
#include <unistd.h>

/* Walltime */
typedef struct timespec walltime_t;

void walltime_start(walltime_t* start)
{
	clock_gettime(CLOCK_MONOTONIC, start);
}

double walltime_elapsed(const walltime_t* start)
{
	walltime_t finish;
	clock_gettime(CLOCK_MONOTONIC, &finish);

	double elapsed = (finish.tv_sec - start->tv_sec);
	elapsed += (finish.tv_nsec - start->tv_nsec) / 1000000000.0;

	return elapsed;
}

/* Needed functions */
long double function(long double x)
{
	return (x * x) + 10 * x + 100;
}

long double disc(long double a, long double b, long double n)
{
	return ( b - a ) / n;
}

long double xi(long double  i, long double a, long double disc)
{
	return a + ( i *  disc );
}

void calculate(long double * results, long double a, long double n, long double disc) {
	int thread_number = omp_get_thread_num();
  	int thread_count = omp_get_num_threads();
	int work = (int) n / thread_count;
	results[thread_number] = 0;
	size_t last = ( (thread_number + 1) * work );
	if (thread_number == thread_count){
		last = n;
	}

	for (size_t index = (thread_number * work) + 1; index < last; ++index){
		results[thread_number] += 2 * function( xi( index, a, disc ) );
	}
}

int main(int argc, char** argv)
{

  /* Initialization with default values */
  long double a = 0;
  long double b = 1;
  long double n = 1000;
	size_t thread_count = omp_get_max_threads();

  /* Process arguments */
  if ( argc == 4 || argc == 5 )
  {
	  if ( sscanf (argv[1], "%Lf", &a) != 1 ) {
		  fprintf(stderr, "error: not a valid value for a. Using default value\n");
	  }
	  if ( sscanf (argv[2], "%Lf", &b) != 1 ) {
		  fprintf(stderr, "error: not a valid value for b. Using default value\n");
	  }
	  if ( sscanf (argv[3], "%Lf", &n) != 1 ) {
		  fprintf(stderr, "error: not a valid value for n. Using default value\n");
	  }
		if ( argc == 5) {
			if ( sscanf (argv[4], "%zd", &thread_count) != 1 ) {
				fprintf(stderr, "error: not a valid value for workers. Using default value\n");
			}
		}
  }

  /* Start calculation */
  walltime_t start;
  walltime_start(&start);

  long double disc_x = disc( a, b, n );
  long double result = function( xi( 0, a, disc_x ) );
  long double results[thread_count];

  #pragma omp parallel num_threads(thread_count)
  calculate(results, a, n, disc_x);

  for (size_t index = 0; index < thread_count; ++index){
  	result += results[index];
  }

  result += function( xi( n, a, disc_x ) );
  result *= (disc_x / 2);

  /* Report results */
  printf( "The aproximate value of the function is: %.9Lf\n", result );
  printf("Elapsed %.9lfs with n = %Lf and %zd workers\n", walltime_elapsed(&start), n, thread_count );

  return 0;
}
