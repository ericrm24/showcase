CREATE PROCEDURE [dbo].[ObtenerOpcionesSeleccionadas]
	@codFormulario VARCHAR(8),
	@correo VARCHAR(50),
	@sigla VARCHAR(10),
	@num TINYINT,
	@semestre TINYINT,
	@anno INT,
	@codSeccion VARCHAR(8),
	@codPregunta VARCHAR(8)
AS
BEGIN
	SELECT * FROM Opciones_seleccionadas_respuesta_con_opciones
	WHERE FCodigo = @codFormulario AND Correo = @correo AND CSigla = @sigla AND GNumero = @num AND GAnno = @anno AND GSemestre = @semestre AND SCodigo = @codSeccion AND PCodigo = @codPregunta;
END;