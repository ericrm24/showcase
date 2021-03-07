#include <pthread.h>
#include <stdio.h>
#include <unistd.h>

typedef struct
{
	pthread_t thread;
	size_t thread_number;
	size_t thread_count;
}thread_info_t;

void thread_init(thread_info_t* this, size_t thread_number, size_t thread_count) 
{
	this->thread_number = thread_number;
	this->thread_count = thread_count;
}

void* hello(void * data)
{
  thread_info_t * thread = (thread_info_t*) data;
  printf("Hello from thread %zu of %zu\n", thread->thread_number, thread->thread_count);
  return NULL;
}

void thread_run(thread_info_t* this) 
{
	pthread_create ( &this->thread, NULL, hello, (void*) this );
}

int main(int argc, char** argv)
{
  size_t thread_count = sysconf( _SC_NPROCESSORS_ONLN ) ;
  if ( argc == 2 )
  {
	  if ( sscanf (argv[1], "%zd", &thread_count) != 1 ) {
		  fprintf(stderr, "error: not an integer\n");
	  }
  }
  
  thread_info_t threads[thread_count];
  for (size_t index = 0; index < thread_count; ++index) {
	  thread_init( &threads[index], index, thread_count );
	  thread_run( &threads[index] );
  }
  
  printf("Hello from main thread\n");
  
  for (size_t index = 0; index < thread_count; ++index) {
	  pthread_join(threads[index].thread, NULL);
  }
  
  return 0;
}
