CREATE TABLE [dbo].[Usuario]
(
	[Username] varchar(50) NOT NULL PRIMARY KEY,
	[Password] varchar(64) NOT NULL,
	[Salt] varchar(64) not null unique, 
    [Activo] BIT NOT NULL DEFAULT 1,
	CONSTRAINT fkUsuarioAPersonaCorreo Foreign key (Username) references Persona(Correo)
	on delete cascade
	on update cascade
)
