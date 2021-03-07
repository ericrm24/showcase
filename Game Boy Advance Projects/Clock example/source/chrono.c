#include "chrono.h"

// Muestra encabezado
void DisplayChrName()
{
    tte_write("#{es}");
    tte_write("#{P:30,35} Chronometer ");
    tte_write("#{P:78,75} :");
    tte_write("#{P:142,75} :");
    /*char str[50] = {0};
    snprintf(str, 50, "#{P:5,7}RTC: %d", RTC);
    tte_write(str);*/
}

// Instrucciones cuando se ejecuta
void DisplayStartMsg()
{
    DisplayChrName();
    tte_write("#{P:1,130} A to pause");
    tte_write("#{P:1,150} UP to stop");
}

// Instrucciones pausado
void DisplayPausedMsg()
{
    DisplayChrName();
    tte_write("#{P:1,130} A to start");
    tte_write("#{P:1,150} UP to stop");
}

// Instrucciones detenido
void DisplayStopMsg()
{
    DisplayChrName();
    tte_write("#{P:1,130} A to start");
}

// Método principal
void Timer(Sprite ** clock, OBJ_ATTR* obj_buffer)
{
    REG_TM0D= -0x4000; // Configurar para overflow correspondiente
    REG_TM0CNT= TM_FREQ_1024;   // 1024  ciclos

    REG_TM1CNT= TM_ENABLE | TM_CASCADE;

    REG_TM0CNT ^= TM_ENABLE;
    REG_TM1CNT ^= TM_ENABLE;

    u32 sec= -1;

    int hour, min, second;
    hour = min = second = 0;
    state_type state = STOP;

    while(1)
    {
        VBlankIntrWait();
        key_poll();

        if(key_hit(KEY_B))
            return;
        
        if(state == START || state == PAUSED)
        {
            if(key_hit(KEY_UP)) //para inicializar el cronómetro
            {
                hour = min = second = 0;
                UpdateClock(clock, hour, min, second);
                REG_TM1CNT ^= TM_ENABLE;
                if(state == PAUSED)
                    REG_TM0CNT ^= TM_ENABLE;
                state = STOP;
            }
        }
        
        if (state == START)
        {
            DisplayStartMsg();
            if(REG_TM1D != sec)
            {
                sec= REG_TM1D;
                hour = sec/3600;
                min = (sec%3600)/60;
                second = sec%60;
                UpdateClock(clock, hour, min, second);
            }

            if (key_hit(KEY_A))
            {
                REG_TM0CNT ^= TM_ENABLE;
                state = PAUSED;
            }
        }
        else if (state == PAUSED)
        {
            DisplayPausedMsg();

            if(key_hit(KEY_A))  // pausa
            {
                REG_TM0CNT ^= TM_ENABLE;
                state = START;
            }
        }
        else if (state == STOP)
        {   
            DisplayStopMsg();
            UpdateClock(clock, 0, 0, 0);
            if (key_hit(KEY_A))
            {
                state = START;
                REG_TM1CNT ^= TM_ENABLE;
            }
        }

        oam_copy(oam_mem, obj_buffer, 6);
    }
}