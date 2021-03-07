CREATE PROCEDURE [dbo].[EliminarPreguntaDeSeccion]
	@SCodigo VARCHAR(8),
	@PCodigo VARCHAR(8)
AS
BEGIN
	DELETE FROM Seccion_tiene_pregunta
	WHERE SCodigo = @SCodigo AND PCodigo = @PCodigo;
END
