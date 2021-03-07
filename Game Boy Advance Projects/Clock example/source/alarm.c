#include "alarm.h"

//Muestra encabezado
void DisplayAlarmName()
{
    tte_write("#{es}");
    tte_write("#{P:30,35} Alarm Setup ");
    tte_write("#{P:78,75} :");
}

//Muestra instrucciones
void DisplayAlarmMsg()
{
    DisplayAlarmName();
    tte_write("#{P:1,130} START to save");
    tte_write("#{P:1,150} B to deactivate");
}

//Muestra encabezado
void DisplayAlertName()
{
    tte_write("#{es}");
    tte_write("#{P:30,35} Alert Active");
    tte_write("#{P:78,75} :");
    tte_write("#{P:142,75} :");
}

//Muestra instrucciones
void DisplayAlertMsg()
{
    DisplayAlarmName();
    tte_write("#{P:1,130} Down to stop alarm");
}

//Comprueba límites de hora y minutos
void CheckLimits(Timestamp* time)
{
    if(time->hour > 23)
        time->hour = 0;

    if(time->hour < 0)
        time->hour = 23;

    if(time->minute > 59)
        time->minute = 0;

    if(time->minute < 0)
        time->minute = 59;
}

//Método que programa la alarma
Timestamp SetAlarm(Sprite** clock, OBJ_ATTR* obj_buffer, Timestamp time)
{
    Timestamp alarm = time;

    bool hourSelected = true;

    int xPos = 0;
    char select[20] = {0};
    char select2[20] = {0};

    while(1)
    {
        VBlankIntrWait();
        key_poll();

        DisplayAlarmMsg();

        UpdateSimpleClock(clock, alarm.hour, alarm.minute);

        xPos = hourSelected ? 45 : 110;

        snprintf(select, 50, "#{P:%d,50} *", xPos);
        tte_write(select); // Mostrar selección
        snprintf(select2, 50, "#{P:%d,100} *", xPos);
        tte_write(select2); // Mostrar selección

        // B para salir y desconfigurar alarma
        if(key_hit(KEY_B))
        {
            alarm.hour = alarm.minute = -1; //alarma con valores en -1 para desactivarla
            return alarm;
        }
        
        // START para guardar configuración
        else if(key_hit(KEY_START))
            return alarm;

        else if(key_hit(KEY_LEFT) || key_hit(KEY_RIGHT))
            hourSelected = !hourSelected;   // Cambiar selección
        
        else if(key_hit(KEY_UP))
        {
            alarm.hour += hourSelected ? 1 : 0;
            alarm.minute += hourSelected ? 0 : 1;

            CheckLimits(&alarm);
        }
        else if(key_hit(KEY_DOWN))
        {
            alarm.hour -= hourSelected ? 1 : 0;
            alarm.minute -= hourSelected ? 0 : 1;

            CheckLimits(&alarm);
        }

        oam_copy(oam_mem, obj_buffer, 6);
    }
}

//Método que produce sonido para la alarma
void Alert()
{
    mmFrame(); // Llamado a música de alarma
}

//Compara la alarma solicitada con el reloj
void CheckAlarm(Timestamp time, Timestamp alarm)
{
    if (time.minute == alarm.minute && time.hour == alarm.hour)
    {
        DisplayAlertMsg();
        Alert();//llama al método que activa sonido
    }
}

