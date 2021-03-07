#include <pthread.h>
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
	size_t * next_print;
	
}thread_t;

typedef struct
{
	thread_t** threads;
	size_t thread_count;
	size_t permission;
	
}thread_info_t;

pthread_mutex_t lock;

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
	this->permission = 0;
	
	for (size_t index = 0; index < thread_count; ++index) {
		this->threads[index] = malloc (sizeof(thread_t) );
		this->threads[index]->id = index;
		this->threads[index]->thread_count = thread_count;
		this->threads[index]->next_print = &this->permission;
	}
	
}

void* hello(void * data)
{
  thread_t * thread = (thread_t*) data;
  while (*thread->next_print < thread->id)
	;
  printf("Hello from thread %zu of %zu\n", thread->id, thread->thread_count);
  
  pthread_mutex_lock (&lock);
  ++*thread->next_print;
  pthread_mutex_unlock (&lock);
  
  return NULL;
}

void thread_run (thread_t* this) {
	pthread_create (&this->thread, NULL, hello, (void*) this);
}

void threads_run(thread_info_t* this) 
{
	for (size_t index = 0; index < this->thread_count; ++index) {
		thread_run(this->threads[index]);
	}
}

void thread_join(thread_info_t* this) {
	for (size_t index = 0; index < this->thread_count; ++index) {
	  pthread_join(this->threads[index]->thread, NULL);
	}
}

void threads_destroy (thread_info_t* this) {
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
  
  pthread_mutex_init (&lock, NULL);
  
  thread_info_t thread_info;
  
  thread_info.threads = malloc (sizeof(thread_t*) * thread_count);
  
  threads_init(&thread_info, thread_count);
  
  walltime_t start;
  walltime_start(&start);
  
  threads_run (&thread_info);
  
  thread_join(&thread_info);
  
  threads_destroy(&thread_info);
  free(thread_info.threads);
  
  pthread_mutex_destroy(&lock);
  
  printf("Elapsed %.9lfs with %lu threads\n", walltime_elapsed(&start), thread_count);
  
  return 0;
}
