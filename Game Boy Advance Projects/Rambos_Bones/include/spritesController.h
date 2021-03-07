
#ifndef SPRITES_CONTROLLER_H
#define SPRITES_CONTROLLER_H

#include <tonc.h>
#include <stdio.h>
#include <string.h>

//Se establecen limites a la hora de mostrar el personaje en pantalla
#define min(x,y) (x > y ? y : x)
#define max(x,y) (x < y ? y : x)

/*========================Estructuras========================*/

//Se define una estructura para contener toda la info que se necesita para mover y animar al personaje
typedef struct
{
	OBJ_ATTR *spriteAttribs;
	int facingRight;
	int firstAnimCycleFrame;
	int animFrame;
    int animCount;
	int posX;
	int posY;
	int velX;
	int velY;
	int framesInAir;
    int isColliding;
    int floor;
    int visible;
}Sprite;

//Estructura para manejar las posiciones de los ejes x y y para la detección de colisiones
typedef struct {
    int x, y;
}Point;

/*========================Constantes=======================*/
extern const int DOG_SIZE; //el personaje es de 32 píxeles.
extern const int ANIM_SPEED; //Velocidad de animaciones
extern const int NUMBER_OF_BONES; //cantidad de huesos en el nivel
extern const int BONE_VALUE; //Puntos ganados al obtener un hueso
extern const int GRAVITY; //la gravedad para caída
extern const int WALK_SPEED; //velocidad de movimiento del personaje
extern const int JUMP_VI; //salto del personaje

/*========================Variables globales========================*/
extern int floorY[3]; //El suelo de cada una de los 3 carriles
extern int score; // Puntuación del jugador

/*========================Funciones========================*/
void InitializeSprite(Sprite* sprite, OBJ_ATTR* attribs, int x, int y);
void updateSpritePosition(Sprite* sprite);
void paint_sprites(Sprite* sprite, int isMidAir, int dog_sprite, int midair_dog_sprite, int resting_dog_sprite);
void tickSpriteAnimation(Sprite* sprite, int c_level);
void updateSpritePositionBone(Sprite* sprite);
void tickSpriteAnimationBone(Sprite* sprite, int c_level);
void noBone(Sprite* sprite);
void animateBones(Sprite** bones, int c_level);
int doOverlap(Sprite *sprite1, Sprite *sprite2);
void checkColisions(Sprite* sprite, Sprite** bones);

#endif // SPRITES_CONTROLLER_H