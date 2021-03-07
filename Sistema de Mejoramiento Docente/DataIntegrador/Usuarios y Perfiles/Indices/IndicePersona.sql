/*Índice sobre las columnas Apellido1 y Apellido2 de la tabla Persona, para que las filas
se desplieguen más rápidamente en las vistas de Usuarios y Perfiles.*/

CREATE INDEX IndicePersona
ON Persona (Apellido1, Apellido2);