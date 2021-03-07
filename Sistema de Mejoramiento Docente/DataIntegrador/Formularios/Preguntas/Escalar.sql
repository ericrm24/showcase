CREATE TABLE Escalar
(
	Codigo VARCHAR(8) NOT NULL PRIMARY KEY,

	-- Hay que validar los defaults
	Incremento INT NOT NULL DEFAULT 1,
	Minimo INT NOT NULL DEFAULT 1,
	Maximo INT NOT NULL DEFAULT 5,
	CONSTRAINT fkEscalarCodigo FOREIGN KEY(Codigo) REFERENCES Pregunta_con_opciones(Codigo) ON DELETE CASCADE ON UPDATE CASCADE,
)
