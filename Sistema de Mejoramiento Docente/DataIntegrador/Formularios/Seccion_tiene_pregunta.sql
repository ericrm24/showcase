CREATE TABLE Seccion_tiene_pregunta
(
	SCodigo VARCHAR(8) NOT NULL,
	PCodigo VARCHAR(8) NOT NULL,
	Orden INT NOT NULL,
	PRIMARY KEY(SCodigo, PCodigo),
	CONSTRAINT fkSeccionTienePreguntaCodigoSeccion FOREIGN KEY(SCodigo) REFERENCES Seccion(Codigo) ON UPDATE CASCADE,
	CONSTRAINT fkSeccionTienePreguntaCodigoPregunta FOREIGN KEY(PCodigo) REFERENCES Pregunta(Codigo) ON UPDATE CASCADE,
)

GO

CREATE TRIGGER [dbo].[ActualizarOrdenPregunta]
	ON [dbo].[Seccion_tiene_pregunta]
	AFTER DELETE
	AS
	BEGIN
		DECLARE @orden INT
		DECLARE @sec VARCHAR(8)

		SELECT @orden = d.[Orden], @sec = d.[SCodigo]
		FROM deleted d;

		UPDATE [Seccion_tiene_pregunta]
		SET Orden = Orden - 1
		WHERE Orden > @orden AND  SCodigo = @sec
	END