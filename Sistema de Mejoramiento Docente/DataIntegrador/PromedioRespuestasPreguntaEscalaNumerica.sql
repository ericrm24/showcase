CREATE PROCEDURE [dbo].[PromedioRespuestasPreguntaEscalaNumerica]
	(@codigoFormulario VARCHAR(8),
	@siglaCurso VARCHAR(50),
	@numeroGrupo TINYINT,
	@anno INT,
	@semestre TINYINT,
	@fechaInicio DATE,
	@fechaFin DATE,
	@codigoSeccion VARCHAR(8),
	@codigoPregunta VARCHAR(8),
	@promedio FLOAT OUTPUT)
AS
	BEGIN
		SELECT @promedio =	AVG(CAST (O.OpcionSeleccionada AS FLOAT))
		FROM Respuestas_a_formulario AS R JOIN Opciones_seleccionadas_respuesta_con_opciones AS O
		ON R.FCodigo = O.FCodigo AND R.Correo = O.Correo AND R.CSigla = O.CSigla AND R.GNumero = O.GNumero AND R.GAnno = O.GAnno AND R.GSemestre = O.GSemestre AND R.Fecha = O.Fecha
		WHERE O.FCodigo = @codigoFormulario
			AND O.CSigla = @siglaCurso
			AND O.GNumero = @numeroGrupo
			AND O.GAnno = @anno
			AND O.GSemestre = @semestre
			AND O.SCodigo = @codigoSeccion
			AND O.PCodigo = @codigoPregunta
			AND O.Fecha >= @fechaInicio
			AND O.Fecha <= @fechaFin
			AND R.Finalizado = 1
	END
RETURN 0
