#include <stdio.h>
#include <stdlib.h>
#include <omp.h>
#include <time.h>
#include <unistd.h>

static size_t workers = 0;

void serial_odd_even_sort(size_t n, double arr[n])
{
  double temp = 0;
  double arr_copy[n];

  #pragma omp parallel for default(none) private(temp) shared (arr_copy, n, arr, workers) num_threads(workers)
  for ( size_t phase = 0; phase < n; ++phase )
  {
    /* Copy array */
    #pragma omp parallel for default (none) shared (arr_copy, arr, n) num_threads(workers)
    for(size_t index = 0; index < n; ++index)
    {
      arr_copy[index] = arr[index];
    }

    if ( phase % 2 == 0 )
  	{
  		for ( size_t i = 1; i < n; i += 2 )
  			if ( arr_copy[i - 1] > arr_copy[i] ){
          temp = arr_copy[i - 1];
          arr[i - 1] = arr_copy[i];
          arr[i] = temp;
        }
  	}
  	else
  	{
  		for ( size_t i = 1; i < n - 1; i += 2 )
  			if ( arr_copy[i] > arr_copy[i + 1] ){
          temp = arr_copy[i + 1];
          arr[i + 1] = arr_copy[i];
          arr[i] = temp;
        }
  	}
  }
}

int main(int argc, char* argv[])
{
  size_t n = 10;
  workers = omp_get_max_threads();

  if (argc >= 2){
    if ( sscanf (argv[1], "%zd", &n) != 1 ) {
      fprintf(stderr, "error: not a valid value for n. Using default value: 10\n");
    }
  }
  if (argc >= 3){
    if ( sscanf (argv[2], "%zd", &workers) != 1 ){
      fprintf(stderr, "error: not a valid value for workers. Using default value: %zu\n", workers);
    }
  }

  // Generate array of size n with double values
  double* arr = malloc(sizeof(double) * n);
  srand(time(NULL));

  #pragma omp parallel for default(none) shared(arr, n) num_threads(workers)
  for (size_t index = 0; index < n; ++index){
    arr[index] = rand() / (double) rand();
  }

  serial_odd_even_sort(n, arr);

  // Print result (only for testing)
  /*for (size_t index = 0; index < n; ++index){
    printf("%f ", arr[index]);
  }
  printf("\n");*/

  free(arr);
}
