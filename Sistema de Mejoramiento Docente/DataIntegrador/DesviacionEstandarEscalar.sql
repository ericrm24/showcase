CREATE PROCEDURE [dbo].[DesviacionEstandarEscalar]
(
	@FCod VARCHAR(8),
	@CSigla VARCHAR(50),
	@Grupo TINYINT,
	@GAnno INT,
	@GSem TINYINT,
	@FechaInicio DATE,
	@FechaFin DATE,
	@SCod VARCHAR(8),
	@PCod VARCHAR(8),
	@Desviacion Float output
)
AS
BEGIN
	SELECT @Desviacion = STDEV(CAST(OpcionSeleccionada AS FLOAT))
	FROM Respuestas_a_formulario AS R JOIN Opciones_seleccionadas_respuesta_con_opciones AS OSRCO
	ON R.FCodigo = OSRCO.FCodigo AND R.Correo = OSRCO.Correo AND R.CSigla = OSRCO.CSigla AND R.GNumero = OSRCO.GNumero AND R.GAnno = OSRCO.GAnno AND R.GSemestre = OSRCO.GSemestre AND R.Fecha = OSRCO.Fecha
			 WHERE OSRCO.FCodigo	= @FCod 
			 AND OSRCO.CSigla		= @CSigla
			 AND OSRCO.GNumero	= @Grupo
			 AND OSRCO.GAnno		= @GAnno
			 AND OSRCO.GSemestre	= @GSem
			 AND OSRCO.SCodigo	= @SCod
			 AND OSRCO.PCodigo	= @PCod
			 AND OSRCO.Fecha >= @FechaInicio
			 AND OSRCO.Fecha <= @FechaFin
			 AND R.Finalizado = 1
END
RETURN 0
