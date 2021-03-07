#include <pthread.h>
#include <stdio.h>
#include <unistd.h>

void* hello(void * data)
{
  size_t thread_number = (size_t)data;
  printf("Hello from thread %zu\n", thread_number);
  return NULL;
}

int main(int argc, char** argv)
{
  size_t thread_count = sysconf( _SC_NPROCESSORS_ONLN ) ;
  if ( argc == 2 )
  {
	  if ( sscanf (argv[1], "%zd", &thread_count) != 1 ) {
		  fprintf(stderr, "error: not an integer, using default value of threads\n");
	  }
  }
  
  pthread_t threads[thread_count];
  for (size_t index = 0; index < thread_count; ++index) {
	  pthread_create( &threads[index], NULL, hello, (void*) index );
  }
  
  printf("Hello from main thread\n");
  
  for (size_t index = 0; index < thread_count; ++index) {
	  pthread_join(threads[index], NULL);
  }
  
  return 0;
}
