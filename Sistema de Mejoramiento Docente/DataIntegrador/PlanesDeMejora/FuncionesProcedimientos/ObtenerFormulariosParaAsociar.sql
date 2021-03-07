CREATE PROCEDURE [dbo].[ObtenerFormulariosParaAsociar]
AS
	SELECT DISTINCT f.FCodigo, f.CSigla, f.GNumero, f.GAnno, f.GSemestre
	FROM Respuestas_a_formulario f
	WHERE f.Finalizado = 0
	ORDER BY f.CSigla, f.GNumero, f.GAnno, f.GSemestre DESC
GO;
