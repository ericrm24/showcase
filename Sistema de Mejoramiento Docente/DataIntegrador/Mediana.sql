CREATE PROCEDURE [dbo].[Mediana]
(
	@codigoFormulario varchar(8),
	@siglaCurso varchar(10),
	@numeroGrupo tinyint,
	@anio int,
	@semestre tinyint,
	@fechaInicio date,
	@fechaFin date,
	@codigoSeccion varchar(8),
	@codigoPregunta varchar(8),
	@mediana FLOAT OUTPUT
)
AS
BEGIN
	SELECT @mediana =
		CAST (PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY o.OpcionSeleccionada)
			OVER (PARTITION BY o.FCodigo) AS FLOAT)
	FROM Respuestas_a_formulario AS r JOIN Opciones_seleccionadas_respuesta_con_opciones o
	ON r.FCodigo = o.FCodigo AND r.Correo = o.Correo AND r.CSigla = o.CSigla AND r.GNumero = o.GNumero AND r.GAnno = o.GAnno AND r.GSemestre = o.GSemestre AND r.Fecha = o.Fecha
	WHERE o.FCodigo = @codigoFormulario and
		  o.CSigla = @siglaCurso and
		  o.GNumero = @numeroGrupo and
		  o.GAnno = @anio and
		  o.GSemestre = @semestre and
		  o.SCodigo = @codigoSeccion and
		  o.PCodigo = @codigoPregunta and
		  o.Fecha >= @fechaInicio and
		  o.Fecha <= @fechaFin and
		  r.Finalizado = 1
	ORDER BY o.FCodigo
END
RETURN 0
