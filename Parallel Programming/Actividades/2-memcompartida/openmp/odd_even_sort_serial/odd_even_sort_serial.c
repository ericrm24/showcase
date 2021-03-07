#include <stdio.h>
#include <stdlib.h>
#include <omp.h>
#include <time.h>
#include <unistd.h>

void serial_odd_even_sort(size_t n, double arr[n])
{
  double temp = 0;
  for ( size_t phase = 0; phase < n; ++phase )
  {
  	if ( phase % 2 == 0 )
  	{
  		for ( size_t i = 1; i < n; i += 2 )
  			if ( arr[i - 1] > arr[i] ){
          temp = arr[i - 1];
          arr[i - 1] = arr[i];
          arr[i] = temp;
        }
  	}
  	else
  	{
  		for ( size_t i = 1; i < n - 1; i += 2 )
  			if ( arr[i] > arr[i + 1] ){
          temp = arr[i + 1];
          arr[i + 1] = arr[i];
          arr[i] = temp;
        }
  	}
  }
}

int main(int argc, char* argv[])
{
  size_t n = 10;

  if (argc == 2){
    if ( sscanf (argv[1], "%zd", &n) != 1 ) {
      fprintf(stderr, "error: not a valid value for n. Using default value: 10\n");
    }
  }

  // Generate array of size n with double values
  double* arr = malloc(sizeof(double) * n);
  srand(time(NULL));
  for (size_t index = 0; index < n; ++index){
    arr[index] = rand() / (double) rand();
  }

  serial_odd_even_sort(n, arr);

  // Print result (only for testing)
/*  for (size_t index = 0; index < n; ++index){
    printf("%f ", arr[index]);
  }
  printf("\n");
*/

  free(arr);
}
