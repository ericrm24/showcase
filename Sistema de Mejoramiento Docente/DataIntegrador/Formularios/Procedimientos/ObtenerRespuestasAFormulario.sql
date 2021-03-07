-- Procedimiento almacenado que devuelve los campos de un intento de respuestas a formulario

CREATE PROCEDURE [dbo].[ObtenerRespuestasAFormulario]
	@codFormulario VARCHAR(8),
	@correo VARCHAR(50),
	@sigla VARCHAR(10),
	@num TINYINT,
	@anno INT,
	@semestre TINYINT
AS
BEGIN
	SELECT * FROM Respuestas_a_formulario
	WHERE FCodigo = @codFormulario AND Correo = @correo AND CSigla = @sigla AND GNumero = @num AND GAnno = @anno AND GSemestre = @semestre;
END
