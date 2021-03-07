//======================================================================
//                              ROCKY'S BONES
//	CI-0155 Sistemas Empotrados de Tiempo Real
//  Proyecto I: Juego pequeño para el emulador GBA usando TONC
//  
//	Integrantes del equipo:
//	   - María José Aguilar
//     - María Jesús Chaves
//	   - Johan Córdoba
//     - Cristy Navarro
//     - Eric Ríos
//                                               II ciclo - 2020
//======================================================================
#include "spritesController.h"
#include "doggy.h"
#include "level1.h"
#include "level2.h"
#include "startScreen.h"
#include "FinalScreen.h"
#include "start2.h"

//===========includes para audio===========
#include <maxmod.h> 
#include <stdlib.h>
#include "sound.h"
#include "sound_bin.h"
//========================================

#define TIME_OVERFLOW -1

/*========================Constantes========================*/
//Se crea un búfer de entradas de OAM que se puede modificar en cualquier momento y copiarlo en el OAM real durante VBlank
// OBJ_ATTR es una estructura para atributos de sprites regulares
OBJ_ATTR obj_buffer[128];

//Se definen y establecen los valores de las constantes que requiere el juego

//Velocidad de parpadeo para pantalla de inicio
const int BLINK_SPEED = 2500;
const int BLINK_CHANGE = (BLINK_SPEED / 2) - 1;
const int LEVELS = 2; // Total de niveles disponibles

//Variable para la interrupción de la DMA
int copy_completed = 0;

//Se define un enum con los posibles estados del juego
typedef enum {
    START,
    PLAYING,
    DONE
} state_type;

/*========================Fondo========================*/

//Carga la pantalla de inicio del juego
void loadStartScreen()
{
    //Cargar datos de la pantalla de inicio
    dma3_cpy(pal_bg_mem, startScreenPal, startScreenPalLen);
    dma3_cpy(&tile_mem[1][0], startScreenTiles, startScreenTilesLen);
    dma3_cpy(&se_mem[30][0], startScreenMap, startScreenMapLen);
}

//Carga la pantalla de transición entre niveles
void loadPassScreen()
{
    //Cargar datos de la pantalla de inicio
    //levels_passTiles
    dma3_cpy(pal_bg_mem, start2Pal, start2PalLen);
    dma3_cpy(&tile_mem[1][0], start2Tiles, start2TilesLen);
    dma3_cpy(&se_mem[30][0], start2Map, start2MapLen);
 
}

//Carga la pantalla de final del juego
void loadFinalScreen()
{
    dma3_cpy(pal_bg_mem, FinalScreenPal,FinalScreenPalLen );
    dma3_cpy(&tile_mem[1][0], FinalScreenTiles, FinalScreenTilesLen);
    dma3_cpy(&se_mem[30][0], FinalScreenMap, FinalScreenMapLen);
}

//Carga la pantalla del juego
void loadBackground()
{
    //Cargar datos del background
    dma3_cpy(pal_bg_mem, level1Pal, level1PalLen);
    dma3_cpy(&tile_mem[1][0], level1Tiles, level1TilesLen);
    dma3_cpy(&se_mem[30][0], level1Map, level1MapLen);
}

void loadBackground2()
{
    //Cargar datos del background
    dma3_cpy(pal_bg_mem, level2Pal, level2PalLen);
    dma3_cpy(&tile_mem[1][0], level2Tiles, level2TilesLen);
    dma3_cpy(&se_mem[30][0], level2Map, level2MapLen);
}

//Limpia los datos de la pantalla inicial
void clearStartScreen()
{
    memset(pal_bg_mem, 0, startScreenPalLen);
    memset(&tile_mem[1][0], 0, startScreenTilesLen);
    memset(&se_mem[30][0], 0, startScreenMapLen);
}

/*========================Audio========================*/

// Toca una nota y muestra cuál se tocó
void note_play(int note, int octave)
{
    // Toca la nota actual
    REG_SND1FREQ = SFREQ_RESET | SND_RATE(note, octave);
}

//Este método inicializa las variables necesarias para poder resproducir sonidos
void configureAudio(){
    // enciende el sonido
    REG_SNDSTAT= SSTAT_ENABLE;
    // sonido en ambos parlantes a full
    REG_SNDDMGCNT = SDMG_BUILD_LR(SDMG_SQR1, 7);
    // DMG ratio a 100%
    REG_SNDDSCNT= SDS_DMG100;
    // no se limpia 
    REG_SND1SWEEP= SSW_OFF;
    // envolvente: vol = 12, decaimiento, tiempo de paso máximo (7); 50% de poder
    REG_SND1CNT= SSQR_ENV_BUILD(12, 0, 7) | SSQR_DUTY1_2;
    REG_SND1FREQ= 0;
}

//Este método inicializa las variables necesarias para poder resproducir la música del juego
void playMusic(){
    irqInit();
    irqSet( IRQ_VBLANK, mmVBlank );
    irqEnable(IRQ_VBLANK);
    mmInitDefault( (mm_addr)sound_bin, 8 );
    mmStart( MOD_MUSIC, MM_PLAY_LOOP );
}
//Este método cambia la música del juego para el nivel 2
void changeMusic(){
    mmStop();
    VBlankIntrWait(); //para reproducir la musica del juego
    mmFrame(); //para reproducir la musica del juego
    mmStart(MOD_MUSIC2, MM_PLAY_LOOP );
}

//Método que actualiza el puntaje en pantalla cuando se come un hueso
void updateScore()
{
    char str[50] = {0};
    snprintf(str, 50, "#{P:5,7}SCORE: %d", score); 
    tte_write(str);
}

//Método que inicia la ejecución del juego
void play(Sprite* sprite, Sprite** bones, int c_level)
{
    VBlankIntrWait(); //mejor alternativa al vid_vsync()
    mmFrame(); //para reproducir la musica del juego
    
    checkColisions(sprite, bones);  //Verifica si se realizan colisiones

    key_poll(); //actualiza el estado en que están las teclas
    
    updateSpritePosition(sprite); //Llama a método que da posición a Rambo
    tickSpriteAnimation(sprite, c_level);  //Llama a método que se encarga de pintar a Rambo en pantalla

    animateBones(bones, c_level); //Llama a método que da la animación del hueso

    oam_copy(oam_mem, obj_buffer, 6); //Se actualiza la verdadera OAM (Memoria de atributos de objeto)

    updateScore(); //Llama a método que mantiene actualizando el marcador
}

int count = 0;

void initClock(){
    REG_TM2D = -0x4000; // 0xFFFFC000
    REG_TM2CNT = TM_FREQ_1024 | TM_ENABLE;
    REG_TM3CNT = TM_ENABLE | TM_CASCADE;
}

u32 sec = TIME_OVERFLOW;
int lastCurrentTime = 0;
void updateClock(){
    if(key_hit(KEY_A)){
        REG_TM2CNT ^= TM_ENABLE;
    }
    if(key_hit(KEY_B)){
        REG_TM2CNT ^= TM_CASCADE;
    }

    if(REG_TM3D != sec){
        sec = REG_TM3D;
        char buf[50] = {};
        snprintf(buf, 50, "#{P:210,7} %02d", sec%60);
        tte_write(buf);
    }
}

void init_sprites(Sprite* bone, Sprite** bones)
{
    InitializeSprite(&bone[0], &obj_buffer[1], 40, floorY[0]); // Inicializa el hueso en posición indicada por parámetros
    InitializeSprite(&bone[1], &obj_buffer[2], 80, floorY[0]); // Inicializa el hueso en posición indicada por parámetros
    InitializeSprite(&bone[2], &obj_buffer[3], 100, floorY[0] - 23); // Inicializa el hueso en posición indicada por parámetros
    InitializeSprite(&bone[3], &obj_buffer[4], 140, floorY[0] - 46); // Inicializa el hueso en posición indicada por parámetros
    InitializeSprite(&bone[4], &obj_buffer[5], 180, floorY[0] - 23); // Inicializa el hueso en posición indicada por parámetros
    bones[0] = &bone[0]; bones[1] = &bone[1]; bones[2] = &bone[2]; bones[3] = &bone[3]; bones[4] = &bone[4];
}

void init_screen()
{
    //Preparar background 1 (del juego) y background 0 (texto)
    REG_BG1CNT = BG_CBB(1) | BG_SBB(30) | BG_8BPP | BG_REG_64x32;
    tte_init_se_default(0, BG_CBB(0) | BG_SBB(24));
    
    //Ya que se exportaron nuestros datos de sprites, los introducimos en VRAM
    //tile_mem [4][0] apunta al primer tile en VRAM. Se carga el sprite en los primeros 64 tiles de VRAM
    dma3_cpy(&tile_mem[4][0], doggyTiles, doggyTilesLen);
    dma3_cpy(pal_obj_mem, doggyPal, doggyPalLen);

    //Inicialización de todos los sprites
    oam_init(obj_buffer, 128); // Se ocultan todos los sprites
    //Se establece el control de display con backgrounds 0 y 1
    REG_DISPCNT= DCNT_MODE0 | DCNT_BG1 | DCNT_BG0;
}
//============Interrupciones==================
void dma3_handler(){
    copy_completed = 1;
}

void initIrq(){
    irq_init(NULL);
    irq_add(II_DMA3, dma3_handler);
}
int main()
{   
	initIrq(); //se activa el registro para manejar las interrupciones
    REG_DMA3CNT |= DMA_IRQ | DMA_ENABLE; //se habilitan las interrupciones de la dma
	
    loadStartScreen();  //Carga la pantalla de inicio
    init_screen();
    
    int current_level = 1;
    int time_ctl = 0;

    Sprite sprite; // Sprite de Rambo
    
    InitializeSprite(&sprite, &obj_buffer[0], 0, floorY[0]);

    Sprite* bones[5];
    Sprite bone[5];
    init_sprites(bone, bones);

    Sprite* chikens[5];
    Sprite chiken[5];
    init_sprites(chiken, chikens);
   
   //Para que el juego inicie hasta que todas las tranferencias a la dma terminen
   while(!copy_completed){   
    }
   
    // Estado de juego para pantalla de inicio
    state_type game_state = START;
    int blinkFrame = 0;

    configureAudio(); //se llama método para configurar las variables 
    playMusic();//se llama método para habilitar la funcionalidad de reproducir la música

	while(1)
	{
        if (game_state == START)
        {
            VBlankIntrWait(); //para reproducir la musica del juego
            mmFrame(); //para reproducir la musica del juego
            // Mostrar pantalla inicial
            if( (blinkFrame % BLINK_SPEED) < BLINK_CHANGE )
            {
                tte_write("#{P:50,130}");
                tte_write("Press A to start");
            }
            else
            {
                tte_write("#{es}");
            }
            ++blinkFrame;

            // Esperar que se presione A para comenzar
            key_poll();
            if (key_released(KEY_A))
            {
                note_play(NOTE_A, 0); //para que suene una nota al presionar la tecla
                game_state = PLAYING;
                tte_write("#{es}");
                clearStartScreen();
                if (current_level == 1)
                {
                    loadBackground();
                }
                else if (current_level == 2)
                {
                    loadBackground2();

                }
                //Se establece el control de display en modo mapeo 1D y con backgrounds 0 y 1
                REG_DISPCNT= DCNT_OBJ | DCNT_OBJ_1D | DCNT_BG0 | DCNT_BG1;
            }
        }
        else if (game_state == PLAYING)
        {
            if (current_level == 1)
            {
                play(&sprite, bones, current_level);
                if (score >= BONE_VALUE * NUMBER_OF_BONES)
                {
                    current_level++;
                    clearStartScreen();
                    loadPassScreen();
                    game_state = START;
                    changeMusic();
                }
            }
            else if (current_level == 2)
            {
                play(&sprite, chikens, current_level);
                VBlankIntrWait(); //para reproducir la musica del juego
                mmFrame(); //para reproducir la musica del juego
                if (!time_ctl)
                {
                    initClock();
                    time_ctl = 0;
                }
                updateClock();

                if (sec > 9)
                {
                    game_state = DONE;
                }
            }
            
        } else {
            VBlankIntrWait(); //para reproducir la musica del juego
            mmFrame(); //para reproducir la musica del juego
            loadFinalScreen();
        }
	}
	return 0;
}