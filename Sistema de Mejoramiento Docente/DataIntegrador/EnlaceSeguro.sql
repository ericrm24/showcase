CREATE TABLE [dbo].[EnlaceSeguro]
(
	[Hash] varchar(64) NOT NULL PRIMARY KEY,
	[UsuarioAsociado] VARCHAR(50) NULL,
	[UrlReal] varchar(1000) Not null,
	[Expira] datetime null DEFAULT dateadd(hour,48,getdate()),
	[Usos] int not null DEFAULT 1,
	[ReestablecerContrasenna] bit not null DEFAULT 0
	CONSTRAINT fkEnlaceSeguroAPersonaCorreo Foreign key (UsuarioAsociado) references Persona(Correo)
)
