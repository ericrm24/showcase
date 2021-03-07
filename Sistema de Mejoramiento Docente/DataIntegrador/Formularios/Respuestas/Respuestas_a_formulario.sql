CREATE TABLE Respuestas_a_formulario
(
	FCodigo VARCHAR(8) NOT NULL,
	Correo VARCHAR(50) NOT NULL DEFAULT 'admin@mail.com',
	CSigla VARCHAR(10) NOT NULL,
	GNumero TINYINT NOT NULL,
	GAnno INT NOT NULL,
	GSemestre TINYINT NOT NULL,
	Fecha DATE NOT NULL,
	Finalizado BIT NOT NULL DEFAULT 0

	PRIMARY KEY(FCodigo, Correo, CSigla, GNumero, GAnno, GSemestre, Fecha),
	CONSTRAINT fkRespuestasFormulario FOREIGN KEY(FCodigo) REFERENCES Formulario(Codigo) on delete cascade,
	CONSTRAINT fkRespuestasUsuario FOREIGN KEY(Correo) REFERENCES Persona(Correo) on delete cascade,
	CONSTRAINT fkRespuestasGrupo FOREIGN KEY(CSigla, GNumero, GSemestre, GAnno) REFERENCES Grupo(SiglaCurso, NumGrupo, Semestre, Anno) on delete cascade,
)
