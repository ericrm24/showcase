CREATE PROCEDURE [dbo].[ObtenerPreguntasDeSeccion]
	@sectionCode varchar(8)
AS
BEGIN
	SELECT p.Codigo, p.Enunciado, p.Tipo, sp.Orden
	FROM Seccion_tiene_pregunta sp JOIN Pregunta p ON sp.PCodigo = p.Codigo
	WHERE sp.SCodigo = @sectionCode
	ORDER BY sp.Orden ASC;
END;
