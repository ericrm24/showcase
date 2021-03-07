CREATE TABLE [dbo].[Pertenece_a]
(
	[CodCarrera] VARCHAR(10) NOT NULL,
	[CodEnfasis] VARCHAR(10) NOT NULL,
	[SiglaCurso] VARCHAR(10) NOT NULL,
	PRIMARY KEY (CodCarrera,CodEnfasis,SiglaCurso),
	FOREIGN KEY (CodCarrera, CodEnfasis) REFERENCES Enfasis (CodCarrera, Codigo) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY (SiglaCurso) REFERENCES Curso (Sigla) ON UPDATE CASCADE ON DELETE CASCADE
)
