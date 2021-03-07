CREATE TABLE Pregunta_con_opciones
(
	Codigo VARCHAR(8) NOT NULL PRIMARY KEY,
	TituloCampoObservacion NVARCHAR(50),
	CONSTRAINT fkPregOpcionesCodigo FOREIGN KEY(Codigo) REFERENCES Pregunta(Codigo) ON DELETE CASCADE ON UPDATE CASCADE,
)
