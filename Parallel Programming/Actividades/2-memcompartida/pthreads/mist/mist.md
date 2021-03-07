#Mist

###Rastreo de memoria del programa:
<img src="./memoria.png" alt="memory" width="800" height="800"/>

###Procesamiento
<img src="./procesamiento.png" alt="proc" width="800" height="800"/>

##Función mistery()
La función mistery actúa como un pthread_barrier. Haciendo uso de pthread_cond_var se asegura de que todos los threads hayan llegado al mismo punto antes de permitirles continuar su ejecución.