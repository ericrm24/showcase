CREATE TABLE [dbo].[Empadronado_en]
(
	[CorreoEstudiante] VARCHAR(50) NOT NULL,
	[CodCarrera] VARCHAR(10) NOT NULL,
	[CodEnfasis] VARCHAR(10) NOT NULL,
	PRIMARY KEY (CorreoEstudiante, CodCarrera, CodEnfasis),
	FOREIGN KEY (CorreoEstudiante) REFERENCES Estudiante (Correo) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY (CodCarrera, CodEnfasis) REFERENCES Enfasis (CodCarrera, Codigo) ON UPDATE CASCADE ON DELETE CASCADE
)
