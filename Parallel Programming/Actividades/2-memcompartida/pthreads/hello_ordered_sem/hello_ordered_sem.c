#include <pthread.h>
#include <semaphore.h>
#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <unistd.h>

typedef struct timespec walltime_t;

typedef struct
{
	pthread_t thread;
	size_t id;
	size_t thread_count;
	sem_t * semaphores;
	
}thread_t;

typedef struct
{
	thread_t** threads;
	size_t thread_count;
	sem_t* semaphores;
	
}thread_info_t;

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

void threads_init(thread_info_t* this, size_t thread_count) 
{
	this->thread_count = thread_count;
	this->semaphores = malloc (sizeof(sem_t) * thread_count);
	
	for (size_t index = 0; index < thread_count; ++index) {
		this->threads[index] = malloc (sizeof(thread_t) );
		this->threads[index]->id = index;
		this->threads[index]->thread_count = thread_count;
		sem_init(&this->semaphores[index], 0, 0);
		this->threads[index]->semaphores = this->semaphores;
	}
	sem_post(&this->semaphores[0]);
	
}

void* hello(void * data)
{
  thread_t * thread = (thread_t*) data;
  sem_wait(&thread->semaphores[thread->id]);
  
  printf("Hello from thread %zu of %zu\n", thread->id, thread->thread_count);
  
  sem_post(&thread->semaphores[(thread->id+1) % thread->thread_count]);
  
  return NULL;
}

void thread_run (thread_t* this)
{
	pthread_create (&this->thread, NULL, hello, (void*) this);
}

void threads_run(thread_info_t* this) 
{
	for (size_t index = 0; index < this->thread_count; ++index) {
		thread_run(this->threads[index]);
	}
}

void thread_join(thread_info_t* this)
{
	for (size_t index = 0; index < this->thread_count; ++index) {
	  pthread_join(this->threads[index]->thread, NULL);
	}
}

void threads_destroy (thread_info_t* this)
{
	for (size_t index = 0; index < this->thread_count; ++index) {
		free(this->threads[index]);
	}
}

int main(int argc, char** argv)
{
  size_t thread_count = sysconf( _SC_NPROCESSORS_ONLN ) ;
  if ( argc == 2 )
  {
	  if ( sscanf (argv[1], "%zd", &thread_count) != 1 ) {
		  fprintf(stderr, "error: not an integer. Using default value\n");
	  }
  }
  
  thread_info_t thread_info;
  
  thread_info.threads = malloc (sizeof(thread_t*) * thread_count);
  
  threads_init(&thread_info, thread_count);
  
  walltime_t start;
  walltime_start(&start);
  
  threads_run (&thread_info);
  
  thread_join(&thread_info);
  
  threads_destroy(&thread_info);
  free(thread_info.threads);
  free(thread_info.semaphores);
  
  printf("Elapsed %.9lfs with %lu\n", walltime_elapsed(&start), thread_count);
  
  return 0;
}
