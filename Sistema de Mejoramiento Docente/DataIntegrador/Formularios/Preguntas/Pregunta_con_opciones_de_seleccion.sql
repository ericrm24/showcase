CREATE TABLE Pregunta_con_opciones_de_seleccion
(
	Codigo VARCHAR(8) NOT NULL PRIMARY KEY,
	CONSTRAINT fkPregOpcionesSeleccionCodigo FOREIGN KEY(Codigo) REFERENCES Pregunta_con_opciones(Codigo) ON DELETE CASCADE ON UPDATE CASCADE,
)
