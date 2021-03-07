 //======================================================================
//                              RELOJ
//	CI-0155 Sistemas Empotrados de Tiempo Real
//  Proyecto II: Implementación de reloj, con cronómetro y alarma
//  
//	Integrantes del equipo:
//	   - María José Aguilar
//     - María Jesús Chaves
//	   - Johan Córdoba
//     - Cristy Navarro
//     - Eric Ríos
//                                               II ciclo - 2020
//=====================================================================================

#include <stdio.h>
#include <string.h>
#include "chrono.h"
#include "numbers.h"

// Estados para alarma
typedef enum {
    BEGIN,
    FINAL
} state_alarm;

OBJ_ATTR obj_buffer[128];
//Muestra el nombre reloj y los : de la hora digital
void DisplayRTCName()
{
    tte_write("#{es}");
    tte_write("#{P:30,35} Clock ");
    tte_write("#{P:78,75} :");
    tte_write("#{P:142,75} :");
}

//Muestra instrucciones
void DisplayRTCMsg()
{
    DisplayRTCName();
    tte_write("#{P:1,110} RIGHT to set alarm");
    tte_write("#{P:1,120} LEFT for chronometer");
}

//Muestra el reloj en pantalla
Timestamp ShowRealTime(Sprite** clock)
{
    int sec, hour, min, second;
    Timestamp rtc;
    
    sec= REG_TM3D;
    hour = sec/3600;
    min = (sec%3600)/60;
    second = sec%60;

    UpdateClock(clock, hour, min, second);

    rtc.hour = hour;
    rtc.minute = min;
    rtc.second = second;

    return rtc;
}

//Método que da color al background
void Background()
{
    pal_bg_mem[0] = RGB15(255, 255, 255);
}

void SetGBAClock()
{
    tte_init_se_default(0, BG_CBB(0) | BG_SBB(24));
    tte_init_con();
    
    memcpy(&tile_mem[4][0], numbersTiles, numbersTilesLen);
	memcpy(pal_obj_mem, numbersPal, numbersPalLen);
    oam_init(obj_buffer, 128);

    REG_BG1CNT = BG_CBB(1) | BG_SBB(30) | BG_4BPP | BG_REG_64x32;

    REG_DISPCNT= DCNT_OBJ | DCNT_OBJ_1D | DCNT_MODE0 | DCNT_BG1 | DCNT_BG0;

    // Configurar timers
    REG_TM2D= -0x4000;
    REG_TM2CNT= TM_FREQ_1024;

    REG_TM3CNT= TM_ENABLE | TM_CASCADE;

    REG_TM2CNT ^= TM_ENABLE;
}

void RealTimeClock(Sprite** clock, OBJ_ATTR* obj_buffer)
{
    //Inicializar real_time y alarm
    Timestamp real_time, alarm;
    alarm.hour = alarm.minute = -1;
    state_alarm state = BEGIN;

    while(1)
    {
        VBlankIntrWait();
        key_poll();

        DisplayRTCMsg();
        
        real_time = ShowRealTime(clock);
        if(key_hit(KEY_LEFT))
            Timer(clock, obj_buffer);//cronómetro
        
        if(key_hit(KEY_RIGHT)){
            alarm = SetAlarm(clock, obj_buffer, real_time);//método de configurar alarma
            state = BEGIN;//Activar de nuevo alarma
        }
        if(state == BEGIN)
        {
            CheckAlarm(real_time, alarm);//llama a método de alarma
        }
        if(key_hit(KEY_DOWN)) //para inicializar el cronómetro
        {
            state = FINAL;
        }

        oam_copy(oam_mem, obj_buffer, 6);
    }
}

//Método principal
int main()
{
    SetGBAClock();

    // Sprites para hora, minuto, segundo
    Sprite h1, h2, m1, m2, s1, s2;

    Sprite* clock[] = {&h1, &h2, &m1, &m2, &s1, &s2};

    ConfigureAudio(); //se llama método para configurar las variables 
    PlayMusic();//se llama método para habilitar la funcionalidad de reproducir la música

    InitializeClock(clock, obj_buffer);

    Background();

    RealTimeClock(clock, obj_buffer);

    return 0;
}