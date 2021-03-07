CREATE TABLE [dbo].[UsuarioPerfil]
(
	[Usuario] varchar(50) NOT NULL,
	[Perfil] VARCHAR(50) NOT NULL,
	[CodCarrera] VARCHAR(10) NOT NULL,
	[CodEnfasis] VARCHAR(10) NOT NULL,
	PRIMARY KEY (Usuario, Perfil, CodCarrera, CodEnfasis),
	FOREIGN KEY (Usuario) REFERENCES Usuario (Username) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY (Perfil) REFERENCES Perfil (Nombre) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY (CodCarrera, CodEnfasis) REFERENCES Enfasis (CodCarrera, Codigo) ON UPDATE CASCADE ON DELETE CASCADE
)
