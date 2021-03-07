#include "spritesController.h"

/*========================Constantes=======================*/
const int DOG_SIZE = 32; //el personaje es de 32 píxeles.
const int ANIM_SPEED = 16; //Velocidad de animaciones
const int NUMBER_OF_BONES = 5; //cantidad de huesos en el nivel
const int BONE_VALUE = 5; //Puntos ganados al obtener un hueso
const int GRAVITY = 1; //la gravedad para caída
const int WALK_SPEED = 2; //velocidad de movimiento del personaje
const int JUMP_VI = 9; //salto del personaje

/*========================Variables globales========================*/

int floorY[3] = {128,103,78}; //El suelo de cada una de los 3 carriles
int score = 0; // Puntuación del jugador

/*========================Sprites========================*/

//Este método inicializa las variables que serán usadas para hacer que el personaje se mueva en el mapa
void InitializeSprite(Sprite* sprite, OBJ_ATTR* attribs, int x, int y)
{
    sprite->spriteAttribs = attribs;
    sprite->facingRight = 0;
    sprite->firstAnimCycleFrame = 0;
    sprite->animFrame = 0;
    sprite->animCount = 0;
    sprite->posX = x;
    sprite->posY = y;
    sprite->velX = 0;
    sprite->velY = 0;
    sprite->framesInAir = 0;
    sprite->isColliding = 0;
    sprite->floor = 0;
    sprite->visible = 1;
}

/*========================Personaje========================*/

//Este método maneja el movimiento del personaje
void updateSpritePosition(Sprite* sprite)
{
 // Pimero se determina la velocidad del perosnaje para el frame actual y luego se agregan las velocidades a su posición
 // facingRight se usa luego para manejar los sprites horizontalmente y que el personaje pueda mirar y moverse de izq-der, der-izq

    if (key_held(KEY_LEFT))//si la tecla de dirección izq se mantiene presionada
    {
        sprite->facingRight = 1; //el personaje no está mirando a la der
        sprite->velX = -WALK_SPEED; //hace que camine hacia la izq
    }
    else if (key_held(KEY_RIGHT)) //si la tecla de dirección der se mantiene presionada
    {
        sprite->facingRight = 0;//el personaje sí está mirando a la der
        sprite->velX = WALK_SPEED; //hace que camine hacia la der
    }
    else sprite->velX = 0; //el personaje no está siendo movido

    //Para manejar cuando el personaje salta
    int isMidAir = sprite->posY != floorY[sprite->floor]; //se obtiene la posición en el eje y del personaje
               
    if (key_hit(KEY_UP)) //si se presiona la tecla de arriba, el personaje salta y cambia de carril
    {
        if (!isMidAir) //si el personaje no está en el aire
        {
            sprite->velY = -JUMP_VI; //hace que el personaje salte
            sprite->framesInAir = 0;
            if (sprite->floor != 2){
                sprite->floor ++;
            }
        }
    }
    if (key_hit(KEY_DOWN)){ //si se presiona la tecla de abajo, el personaje baja y cambia de carril
        //Buscar otro sonido al bajar
        if(!isMidAir){
            sprite->velY = JUMP_VI; //hace que el personaje salte
            sprite->framesInAir = 0;
            if (sprite->floor != 0){
                sprite->floor --;
            }
        }
    }

    if (isMidAir) //si el personaje está en el aire
    {
        //se hace que el personaje caiga al suelo, con efecto de gravedad
        sprite->velY = -JUMP_VI + (GRAVITY * sprite->framesInAir); 
        sprite->velY = min(5, sprite->velY);
        sprite->framesInAir++;
    }

    sprite->posX += sprite->velX; //se actualiza la posición del personaje en el eje x

    //Hace que se muestre el personaje en la pantalla
    sprite->posX = min(240-DOG_SIZE, sprite->posX);
    sprite->posX = max(0, sprite->posX);

    sprite->posY += sprite->velY;
    sprite->posY = min(sprite->posY, floorY[sprite->floor]);

	sprite->spriteAttribs->attr0 = ATTR0_8BPP | ATTR0_SQUARE ; //8bpp porque tenemos 256 colores y la forma del sprite es cuadrada

	sprite->spriteAttribs->attr1 = ATTR1_SIZE_32; //el tamaño de cada sprite es 32px
	sprite->spriteAttribs->attr1 |= (sprite->facingRight? 0 : ATTR1_HFLIP); //attr1 es quien tiene las coordenadas en x, ATTR1_HFLIP se refiere al volteo horizontal/vertical
	obj_set_pos(sprite->spriteAttribs, sprite->posX, sprite->posY); //se actualizan las coordenadas del personaje
}

void paint_sprites(Sprite* sprite, int isMidAir, int dog_sprite, int midair_dog_sprite, int resting_dog_sprite)
{
    if (isMidAir) //actualiza la velocidad de la gravedad
    {
        sprite->firstAnimCycleFrame = midair_dog_sprite * DOG_SIZE; //7
        sprite->animFrame = sprite->velY > 0 ? 1 : 0;
    }
    else
    {
        sprite->animCount++;
        if (sprite->velX != 0)
        {
            sprite->firstAnimCycleFrame = dog_sprite * DOG_SIZE;//4
            sprite->animCount = (sprite->animCount) % ANIM_SPEED;
            sprite->animFrame = (sprite->animCount) / (ANIM_SPEED / 3); //rota entre los 3 frames

        }
        else
        {
            sprite->firstAnimCycleFrame = resting_dog_sprite * DOG_SIZE;//9
            sprite->animCount = (sprite->animCount) % ANIM_SPEED;
            sprite->animFrame = (sprite->animCount) / (ANIM_SPEED / 2); //rota entre los 2 frames
        }
        
    }
}

//Este método maneja la animación del personaje
void tickSpriteAnimation(Sprite* sprite, int c_level)
{
    OBJ_ATTR* spriteAttribs = sprite->spriteAttribs;

    int isMidAir = sprite->posY != floorY[sprite->floor]; //determina si el personaje está saltando
   
    //firstAnimCycleFrame mantiene el índice en el primer frame de ese ciclo de animación
    //animFrame mantiene el cuadro de animación actual en el que estamos en el ciclo de animación

    if (c_level == 1)
    {
        paint_sprites(sprite, isMidAir, 4,7,9);
    }
    else if (c_level == 2)
    {
        paint_sprites(sprite, isMidAir, 13,16,18);
    }

	spriteAttribs->attr2= ATTR2_BUILD(sprite->firstAnimCycleFrame + (sprite->animFrame*DOG_SIZE), 0, 0); 

}

/*========================Huesos========================*/

//Establece posición del hueso
void updateSpritePositionBone(Sprite* sprite)
{
	sprite->spriteAttribs->attr0 = ATTR0_8BPP | ATTR0_SQUARE ;//Maneja colores y forma que en este caso es cuadrada
    sprite->spriteAttribs->attr1 = ATTR1_SIZE_32;  //tamaño del sprite de hueso
	obj_set_pos(sprite->spriteAttribs, sprite->posX, sprite->posY);//Son las coordenadas del hueso en el eje x y y
}

//Maneja la animación del hueso
void tickSpriteAnimationBone(Sprite* sprite, int c_level)
{
    OBJ_ATTR* spriteAttribs = sprite->spriteAttribs;

    sprite->animCount++;
    if (c_level == 1)
    {
        sprite->firstAnimCycleFrame = 11 * DOG_SIZE;// Posición del hueso en el sprite completo
    }
    else if (c_level == 2)
    {
        sprite->firstAnimCycleFrame = 20 * DOG_SIZE;// Posición del hueso en el sprite completo
    }
    
    //Las animaciones para los huesos van a la mitad de velocidad de los demás
    sprite->animCount = (sprite->animCount) % (ANIM_SPEED * 2);
    sprite->animFrame = (sprite->animCount) / ANIM_SPEED; // toma el siguiente hueso en el sprite
	spriteAttribs->attr2= ATTR2_BUILD(sprite->firstAnimCycleFrame + (sprite->animFrame*DOG_SIZE), 0, 0); 

}

//Maneja la desaparición del hueso
void noBone(Sprite* sprite)
{
    OBJ_ATTR* spriteAttribs = sprite->spriteAttribs;
    sprite->firstAnimCycleFrame = 22 * DOG_SIZE;// Posición del hueso en el sprite completo
	spriteAttribs->attr2= ATTR2_BUILD(sprite->firstAnimCycleFrame, 0, 0); 
}

//Método que maneja la animación de los huesos en pantalla cuando no han colisionado con ellos.
void animateBones(Sprite** bones, int c_level)
{
    for (int index = 0; index < NUMBER_OF_BONES; ++index)
    {
        if (bones[index]->visible)
        {
            updateSpritePositionBone(bones[index]);//Llama a método que da posición del hueso
            tickSpriteAnimationBone(bones[index], c_level); //Llama a método que se encarga de pintarlo en pantalla
        }
    }
}

/*========================Colisiones========================*/

//Maneja las posiciones en el eje x y y del perro y el hueso para detectar cuando colisionan 
int doOverlap(Sprite *sprite1, Sprite *sprite2)
{
    Point l1 = {
        .x = sprite1->posX + 2,
        .y = sprite1->posY,
    };
    Point r1 = {
        .x = sprite1->posX + 14,
        .y = sprite1->posY + 16,
    };
    Point l2 = {
        .x = sprite2->posX + 2,
        .y = sprite2->posY,
    };
    Point r2 = {
        .x = sprite2->posX + 14,
        .y = sprite2->posY + 16
    };


    if (l1.x >= r2.x || l2.x >= r1.x) 
        return 0; 
  
    // si hay un cuadrado sobre otro
    if (l1.y >= r2.y || l2.y >= r1.y) 
        return 0; 
  
    return 1; 
}

//Método que revisa si hay una colisión entre sprites
void checkColisions(Sprite* sprite, Sprite** bones)
{
    for (int index = 0; index < NUMBER_OF_BONES; ++index)
    {
        if (bones[index]->visible && doOverlap(sprite, bones[index]))
        {
            // Quitar hueso
            noBone(bones[index]);
            note_play(NOTE_D, 0); //hace un sonido para indicar colision con el hueso
            //Aumentar puntuación
            score += BONE_VALUE;
            //Ya no debe colisionar
            bones[index]->visible = 0;
        }
    }
}