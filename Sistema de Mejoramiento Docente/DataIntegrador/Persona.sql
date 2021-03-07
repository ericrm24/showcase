CREATE TABLE [dbo].[Persona]
(
	[Correo] VARCHAR(50) NOT NULL PRIMARY KEY, 
	[CorreoAlt] VARCHAR(50) NULL,
	[Identificacion] VARCHAR(30) NOT NULL UNIQUE, 
    [Nombre1] VARCHAR(15) NOT NULL, 
    [Nombre2] VARCHAR(15) NULL, 
    [Apellido1] VARCHAR(15) NOT NULL, 
    [Apellido2] VARCHAR(15) NULL, 
	[TipoIdentificacion] VARCHAR(30) NOT NULL,
	[Borrado] BIT not NULL DEFAULT 0
)