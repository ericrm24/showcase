#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>
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

typedef struct{
    long double a;
    long double n;
    long double disc;
    long double* result_p;
    pthread_barrier_t* barrier;
    pthread_t thread;
		pthread_mutex_t* mutex;
    int first;
    int last;
}threads_t;

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

void* threads_run(void* data)
{
		threads_t* thread = (threads_t*) data;
		long double sum = 0;
    for (int index = thread->first + 1; index < thread->last; ++index){
				sum += 2 * function( xi( index, thread->a, thread->disc ) );
    }

		pthread_mutex_lock(thread->mutex);
		*thread->result_p += sum;
		pthread_mutex_unlock(thread->mutex);

    pthread_barrier_wait(thread->barrier);

    return NULL;
}

int main(int argc, char** argv)
{

  /* Initialization with default values */
  long double a = 0;
  long double b = 1;
  long double n = 1000;
  size_t workers = sysconf( _SC_NPROCESSORS_ONLN ) ;

  /* Process arguments */
  if ( argc == 5 || argc == 4 )
  {
	  if ( sscanf (argv[1], "%Lf", &a) != 1 ) {
		  fprintf(stderr, "error: not a valid value for a. Using default value: 0\n");
	  }
	  if ( sscanf (argv[2], "%Lf", &b) != 1 ) {
		  fprintf(stderr, "error: not a valid value for b. Using default value: 1\n");
	  }
	  if ( sscanf (argv[3], "%Lf", &n) != 1 ) {
		  fprintf(stderr, "error: not a valid value for n. Using default value: 1000\n");
	  }
		if ( argc == 5 ) {
			if ( sscanf (argv[4], "%zu", &workers) != 1 ) {
		  	fprintf(stderr, "error: not a valid value for workers. Using default value: Available CPUs\n");
	  	}
		}
  }

  /* Start calculation */
  walltime_t start;
  walltime_start(&start);

  long double disc_x = disc( a, b, n );
  long double result = function( xi( 0, a, disc_x ) );

  pthread_barrier_t barrier;
	pthread_mutex_t mutex;
  pthread_barrier_init(&barrier, NULL,(unsigned int) workers + 1);
	pthread_mutex_init(&mutex, NULL);
  threads_t threads[workers];
  int work = (int) n / workers;
	int extra = (int) n % workers;

  for (size_t index = 0; index < workers; ++index){
      threads[index].barrier = &barrier;
      threads[index].result_p = &result;
      threads[index].first = (int)index * work;
      threads[index].last = (int)(index + 1) * work;
      threads[index].a = a;
      threads[index].n = n;
			threads[index].mutex = &mutex;
      threads[index].disc = disc_x;
			if (extra){
				if (extra > (int) index){
					threads[index].first += index;
					threads[index].last += index + 1;
				}
				else {
					threads[index].first += extra;
					threads[index].last += extra;
				}
			}
			pthread_create(&threads[index].thread, NULL, threads_run, (void*) &threads[index]);
  }

  pthread_barrier_wait(&barrier);

    for (size_t index = 0; index < workers; ++index){
           pthread_join(threads[index].thread, NULL);
    }

  result += function( xi( n, a, disc_x ) );
  result *= (disc_x / 2);

	pthread_mutex_destroy(&mutex);
	pthread_barrier_destroy(&barrier);

  /* Report results */
  printf( "The aproximate value of the function is: %.9Lf\n", result );
  printf("Elapsed %.9lfs with n = %Lf and %zu worker(s)\n", walltime_elapsed(&start), n, workers );

  return 0;
}
