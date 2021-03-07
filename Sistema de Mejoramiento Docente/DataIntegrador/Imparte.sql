CREATE TABLE [dbo].[Imparte]
(
	[CorreoProfesor] VARCHAR(50) NOT NULL,
	[SiglaCurso] VARCHAR(10) NOT NULL,
	[NumGrupo] TINYINT NOT NULL,
	[Semestre] TINYINT NOT NULL,
	[Anno] INT NOT NULL,
	PRIMARY KEY (CorreoProfesor, SiglaCurso, NumGrupo, Semestre, Anno),
	FOREIGN KEY (CorreoProfesor) REFERENCES Profesor (Correo) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY (SiglaCurso, NumGrupo, Semestre, Anno) REFERENCES Grupo (SiglaCurso, NumGrupo, Semestre, Anno) ON UPDATE CASCADE ON DELETE CASCADE
)
