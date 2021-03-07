# Diseño hot_potato_broadcast

## Registro compartido

* Valor de la papa
* Acción por realizar
* Jugador actual
* Jugadores activos
* Valor inicial de la papa

### Acciones posibles

* Pasar la papa
* Salir del juego
* Ganador encontrado

## Máquina de estados (pseudocódigo)

actualizar jugador

si (se encontró ganador)

* Acabar ejecución

si (jugador actual y sigue jugando)

* Trabajar según la acción

compartir registro

repetir

### Acciones por realizar

si (pasar la papa)

* Transformar papa y avisar al siguiente jugador si perdió

si (salir del juego)

* Reducir la cantidad de jugadores activos y restaurar el valor inicial de la papa

si (jugadores activos = 1)

* Anunciar que es el ganador y cambiar acción a: se encontró ganador