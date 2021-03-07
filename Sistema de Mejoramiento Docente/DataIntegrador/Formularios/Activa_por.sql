CREATE TABLE Activa_por
(
	-- Hay que cambiar varios valores cuando estén las otras tablas.
	FCodigo VARCHAR(8) NOT NULL,
	CSigla VARCHAR(10) NOT NULL,
	GNumero TINYINT NOT NULL,
	GAnno INT NOT NULL,
	GSemestre TINYINT NOT NULL,

	PRIMARY KEY(FCodigo, CSigla, GNumero, GAnno, GSemestre),
	CONSTRAINT fkActivaPorFormulario FOREIGN KEY(FCodigo) REFERENCES Formulario(Codigo),
	CONSTRAINT fkActivaPorGrupo FOREIGN KEY(CSigla, GNumero, GSemestre, GAnno) REFERENCES Grupo(SiglaCurso, NumGrupo, Semestre, Anno),
)