CREATE TABLE Opciones_de_seleccion
(
	Codigo VARCHAR(8) NOT NULL,
	Orden INT NOT NULL,
	Texto NVARCHAR(50) NOT NULL,
	PRIMARY KEY(Codigo, Orden, Texto),
	CONSTRAINT fkOpcionesCodigo FOREIGN KEY(Codigo) REFERENCES Pregunta_con_opciones_de_seleccion(Codigo) ON DELETE CASCADE ON UPDATE CASCADE,
)
