CREATE PROCEDURE [dbo].[ObtenerOpcionesDePregunta]
	@questionCode varchar(8)
AS
BEGIN
	SELECT op.Texto, op.Orden
	FROM Pregunta p JOIN Opciones_de_seleccion op ON p.Codigo = op.Codigo
	WHERE p.Codigo = @questionCode
	ORDER BY op.Orden;
END
