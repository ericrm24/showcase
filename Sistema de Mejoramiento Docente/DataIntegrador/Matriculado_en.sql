CREATE TABLE [dbo].[Matriculado_en]
(
	[CorreoEstudiante] VARCHAR(50) NOT NULL,
	[SiglaCurso] VARCHAR(10) NOT NULL,
	[NumGrupo] TINYINT NOT NULL,
	[Semestre] TINYINT NOT NULL,
	[Anno] INT NOT NULL,
	PRIMARY KEY (CorreoEstudiante, SiglaCurso, NumGrupo, Semestre, Anno),
	CONSTRAINT FK_Matricula_correo FOREIGN KEY (CorreoEstudiante) REFERENCES Estudiante (Correo) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT FK_Matricula_grupo FOREIGN KEY (SiglaCurso, NumGrupo, Semestre, Anno) REFERENCES Grupo (SiglaCurso, NumGrupo, Semestre, Anno) ON UPDATE CASCADE ON DELETE CASCADE
)
GO

-- Creacion del indice para la historia RIP-EDF1
CREATE INDEX indice_correo_matriculado
ON Matriculado_en(CorreoEstudiante);