#include "alarm.h"

// Estados del cronómetro
typedef enum {
    START,
    PAUSED,
    STOP
} state_type;

// Método principal del cronómetro
void Timer(Sprite **, OBJ_ATTR*);