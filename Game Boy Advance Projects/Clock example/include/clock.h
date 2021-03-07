#include <stdio.h>
#include <tonc.h>
#include <string.h>
#include "numbers.h"

//Estructura para el sprite de números
typedef struct
{
	OBJ_ATTR *spriteAttribs;
	int firstAnimCycleFrame;
	int animFrame;
	int x;
	int y;
}Sprite;

typedef struct
{
	int hour;
	int minute;
	int second;
}Timestamp;

//Se inicializa las variables del sprite de números
void InitializeSprite(Sprite*, OBJ_ATTR*, int, int);

//Método que inicializa los atributos de reloj
void InitializeClock(Sprite**, OBJ_ATTR*);

//Método que actualiza el reloj
void UpdateClock(Sprite**, int, int, int);

void UpdateSimpleClock(Sprite**, int, int);