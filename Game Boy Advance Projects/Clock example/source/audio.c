#include "audio.h"

//Inicializa variables para audio
void ConfigureAudio(){
    REG_SNDSTAT= SSTAT_ENABLE;// enciende el sonido
    REG_SNDDMGCNT = SDMG_BUILD_LR(SDMG_SQR1, 7);// sonido en ambos parlantes a full
    REG_SNDDSCNT= SDS_DMG100;// DMG ratio a 100%
    REG_SND1SWEEP= SSW_OFF;// no se limpia 
    REG_SND1CNT= SSQR_ENV_BUILD(12, 0, 7) | SSQR_DUTY1_2;// envolvente: vol = 12, decaimiento, tiempo de paso máximo (7); 50% de poder
    REG_SND1FREQ= 0;
}

//Inicializa variables para reproducir audio
void PlayMusic(){
    irqInit();
    irqSet( IRQ_VBLANK, mmVBlank );
    irqEnable(IRQ_VBLANK);
    mmInitDefault( (mm_addr)sound_bin, 8 );
    mmStart( MOD_MUSIC, MM_PLAY_LOOP );
}