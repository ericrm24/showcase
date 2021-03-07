#include <omp.h>
#include <stdio.h>

void hello(void)
{
  int thread_number = omp_get_thread_num();
  int thread_count = omp_get_num_threads();
  printf("Hello from worker %d of %d\n", thread_number, thread_count);
}

int main(int argc, char** argv)
{
  size_t thread_count = omp_get_max_threads();
  if ( argc == 2 )
  {
	  if ( sscanf (argv[1], "%zd", &thread_count) != 1 ) {
		  fprintf(stderr, "error: not a valid value. Using default\n");
	  }
  }

  //omp_set_num_threads(thread_count);

  #pragma omp parallel num_threads(thread_count)
  hello();

  printf("hello from main thread\n");

  return 0;
}
