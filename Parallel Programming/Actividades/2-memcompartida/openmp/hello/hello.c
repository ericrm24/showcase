#include <omp.h>
#include <stdio.h>

void hello(void)
{
  int thread_number = omp_get_thread_num();
  int thread_count = omp_get_num_threads();
  printf("Hello from worker %d of %d\n", thread_number, thread_count);
}

int main(void)
{
  #pragma omp parallel
  hello();

  printf("hello from main thread\n");

  return 0;
}
