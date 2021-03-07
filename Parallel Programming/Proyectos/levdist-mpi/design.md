# Diseño levdist-mpi

## Asignación dinámica vs. bloques

### Ventajas asignación dinámica

* Mejor distribución del trabajo, considerando su costo de procesamiento
* Podría evitar que un solo proceso acabe con trabajos pesados

### Desventajas asignación dinámica

* Requiere un mayor nivel de diseño
* Implementación de nuevas subrutinas y estructuras para mantener actualizada una cola de trabajos por realizar y a quién pertenece cada trabajo realizado
* Código más difícil de mantener

### Ventajas asignación por bloques

* Implementación más sencilla
* Código entendible y por tanto mantenible
* Seguridad de la pertenencia de cada trabajo a su proceso

### Desventajas asignación por bloques

* Sería posible sobrecargar a un solo proceso
* Distribución de la carga de trabajo no tan buena

Basado en las consideraciones anteriores, se opta por un modelo de asignación por bloques.

## Algoritmo

Aprovechando el código ya implementado, la ejecución de cada proceso será muy similar a la actual:

1. Análisis de argumentos: En caso de requerir mostrar información al usuario, como en el reporte de errores, lo realiza el proceso 0.
2. Levantar el listado de archivos: Cada proceso levanta su propia cola de archivos, y de encontrarse algún error, cada proceso lo indica individualmente. Esta cola acaba siendo la misma en cada proceso.
3. Estructura de comparaciones: Cada proceso levanta su propia estructura de comparaciones, incluso asignando los nombres de los archivos a comparar en cada ocasión.
4. Distribución del trabajo: De acuerdo a su rango dentro del grupo de trabajo, cada proceso calcula un bloque sobre el que tiene que encontrar resultados en la estructura de comparaciones.
5. Integración de resultados: Una vez calculadas las distancias de las comparaciones en su bloque de trabajo, se comparten resultados con todos los demás procesos, recibiendo las distancias de cada bloque del proceso al que pertenecían.
6. El proceso 0 se encarga de ordenar los resultados y mostrarlos al usuario.

El mayor cambio respecto a la versión anterior es que solamente calculan las distancias de las comparaciones que correspondieron según el rango y después deben compartirse los resultados, pero cada proceso tiene su propia cola, estructura de comparaciones, y así por el estilo.

## Ejemplo de ejecución

Presente en el archivo rastreo.ods, con 3 archivos a comparar y 3 procesos.