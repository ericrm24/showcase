#include <pthread.h>
#include <stdio.h>
#include <stdlib.h>

//#include "pthread_barrier.h"

static size_t worker_count = 3;
static size_t number_count = 3;
static size_t workers_done = 0;
static size_t* numbers = NULL;
static size_t* numbers_copy = NULL;
static size_t current_step = 0;
static size_t max_steps = 10;
static pthread_t* workers = NULL;
static pthread_barrier_t barrier;

void* calculate(void* data)
{
    const size_t my_id = (size_t)data;
    const size_t work = (size_t) number_count / worker_count;
    size_t extra = (size_t) number_count % worker_count;
    size_t first = work * my_id;
    size_t last = work * (my_id + 1);

    if (extra){
  		if (extra > my_id){
  			first += my_id;
  			last += my_id + 1;
  		}
  		else {
  			first += extra;
  			last += extra;
  		}
    }

    size_t numbers_done = 0;
    int done = 0;
    int first_time = 1;

    //printf("Threads run \n");

    while ( current_step < max_steps && workers_done < worker_count )
    {
      /* Save previous state */
      for (size_t index = first; index < last; ++index){
          numbers_copy[index] = numbers[index];
      }

      pthread_barrier_wait(&barrier);

      for (size_t index = first; index < last; ++index){
          /* Checks at least once */
          if (numbers[index] > 1 || first_time) {
              if (numbers[index] > 1){
                if ( numbers_copy[index] % 2 == 0 )
                    numbers[index] /= 2;
                else
                    numbers[index] = numbers_copy[(index-1) % number_count] * numbers_copy[my_id] + numbers_copy[(index+1) % number_count];
              }

              if (numbers[index] == 1){
                  ++numbers_done;
              }
          }
        }

        first_time = 0;

        if ( my_id == 0 )
            ++current_step;

        if (numbers_done == last - first && !done){
            done = 1;
            ++workers_done;
        }

        pthread_barrier_wait(&barrier);

      }
    return NULL;
}

int main()
{
    printf("Enter the quantity of numbers:\n");
    scanf("%zu", &number_count);
    printf("Enter the quantity of workers:\n");
    scanf("%zu", &worker_count);
    printf("Enter the numbers one by one:\n");
    numbers = malloc( number_count * sizeof(size_t) );
    numbers_copy = malloc( number_count * sizeof(size_t) );
    for ( size_t index = 0; index < number_count; ++index )
        scanf("%zu", &numbers[index]);

    pthread_barrier_init(&barrier, NULL, (unsigned)worker_count);

    workers = malloc(worker_count * sizeof(pthread_t));
    for ( size_t index = 0; index < worker_count; ++index )
        pthread_create(&workers[index], NULL, calculate, (void*)index);

    for ( size_t index = 0; index < worker_count; ++index )
        pthread_join(workers[index], NULL);

    pthread_barrier_destroy(&barrier);

    if ( workers_done < worker_count )
        printf("No converge in %zu steps\n", max_steps);
    else
        printf("Converged in %zu steps\n", current_step);

    free(workers);
    free(numbers_copy);
    free(numbers);

    return 0;
}
