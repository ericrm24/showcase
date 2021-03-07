CREATE TABLE Responde_respuesta_con_opciones
(
	FCodigo VARCHAR(8) NOT NULL,
	Correo VARCHAR(50) NOT NULL,
	CSigla VARCHAR(10) NOT NULL,
	GNumero TINYINT NOT NULL,
	GAnno INT NOT NULL,
	GSemestre TINYINT NOT NULL,
	Fecha DATE NOT NULL,
	PCodigo VARCHAR(8) NOT NULL,
	SCodigo VARCHAR(8) NOT NULL,

	Justificacion NVARCHAR(250),

	PRIMARY KEY(FCodigo, Correo, CSigla, GNumero, GAnno, GSemestre, Fecha, PCodigo, SCodigo),
	CONSTRAINT fkRespondeOpcionesRespuestasAFormulario FOREIGN KEY(FCodigo, Correo, CSigla, GNumero, GAnno, GSemestre, Fecha) REFERENCES Respuestas_a_formulario(FCodigo, Correo, CSigla, GNumero, GAnno, GSemestre, Fecha) on delete cascade,
	CONSTRAINT fkRespondeOpcionesCodigoPregunta FOREIGN KEY(PCodigo) REFERENCES Pregunta(Codigo) on delete cascade,
	CONSTRAINT fkRespondeOpcionesCodigoSeccion FOREIGN KEY(SCodigo) REFERENCES Seccion(Codigo) on delete cascade,
)