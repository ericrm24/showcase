CREATE TABLE [dbo].[Grupo]
(
	[SiglaCurso] VARCHAR(10) NOT NULL,
	[NumGrupo] TINYINT NOT NULL,
	[Semestre] TINYINT NOT NULL,
	[Anno] INT NOT NULL,
	PRIMARY KEY (SiglaCurso, NumGrupo, Semestre, Anno),
	CONSTRAINT FK_Grupo_sigla FOREIGN KEY (SiglaCurso) REFERENCES Curso (Sigla) ON UPDATE CASCADE ON DELETE CASCADE
)
