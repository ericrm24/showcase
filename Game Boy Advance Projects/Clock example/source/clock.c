#include "clock.h"

const int NUMBER_SIZE = 32;//tamaño del sprite
const int INITIAL_X = 20;//posición en eje x
const int INITIAL_Y = 60;//posición en eje y

//Se inicializa las variables del sprite de números
void InitializeSprite(Sprite* sprite, OBJ_ATTR* attribs, int x, int y)
{
    sprite->spriteAttribs = attribs;
    sprite->firstAnimCycleFrame = 0;
    sprite->animFrame = 0;
    sprite->x = x;
    sprite->y = y;
}

//Método que inicializa los atributos de reloj
void InitializeClock(Sprite** clock, OBJ_ATTR* obj_buffer)
{
    int posX = 0;
    for(int index = 0; index < 6; ++index)
    {
        posX = INITIAL_X + (index * NUMBER_SIZE);
        
        InitializeSprite(clock[index], &obj_buffer[index], posX, INITIAL_Y);

        clock[index]->spriteAttribs->attr0 = ATTR0_8BPP | ATTR0_SQUARE ;
        clock[index]->spriteAttribs->attr1 = ATTR1_SIZE_32;
        clock[index]->spriteAttribs->attr1 |= 0;
        clock[index]->spriteAttribs->attr2= ATTR2_BUILD(0 * NUMBER_SIZE, 0, 0);

        obj_set_pos(clock[index]->spriteAttribs, clock[index]->x, clock[index]->y);
    }
}

//Método que actualiza el reloj
void UpdateClock(Sprite** clock, int hour, int min, int sec)
{
    int time[] = {hour, min, sec};
    int num = 0;
    for(int index = 0; index < 6; ++index)
    {
        // 0,1 -> 0    2,3 -> 1    4,5 -> 3
        num = time[(int) ((index + 1) / 2.5)];
        num = index % 2 ? num % 10 : num / 10;
        // Actualizar número que debe mostrar
        clock[index]->animFrame = num;
        clock[index]->spriteAttribs->attr2 = ATTR2_BUILD(num * NUMBER_SIZE, 0, 0);
    }
}

// Muestra la versión del reloj sin segundos, para configuración de alarma
void UpdateSimpleClock(Sprite** clock, int hour, int min)
{
    int time[] = {hour, min};
    int num = 0;
    for(int index = 0; index < 4; ++index)
    {
        // 0,1 -> 0    2,3 -> 1    4,5 -> 3
        num = time[(int) ((index + 1) / 2.5)];
        num = index % 2 ? num % 10 : num / 10;
        // Actualizar número que debe mostrar
        clock[index]->animFrame = num;
        clock[index]->spriteAttribs->attr2 = ATTR2_BUILD(num * NUMBER_SIZE, 0, 0);
    }

    // No mostrar segundos
    clock[4]->spriteAttribs->attr2 = ATTR2_BUILD(-1 * NUMBER_SIZE, 0, 0);
    clock[5]->spriteAttribs->attr2 = ATTR2_BUILD(-1 * NUMBER_SIZE, 0, 0);
}