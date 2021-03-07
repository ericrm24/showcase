/* 
	* HH.UU.:
	* COD-67: Desplegar la información del puntaje de un profesor y un curso específico.
	* Tarea Técnica: Calcular el promedio agregado.
	*				 Recuperar la cantidad de calificaciones totales.
	*				 Recuperar la cantidad de calificaciones para cada rango.
	* 
	* COD-78: Visualizar un gráfico para la calificación de un profesor 
	* y promedio de un curso, a partir de los filtros.
	* Tarea Técnica: Desplegar la cantidad de calificaciones obtenidas a partir de los filtros.
	* 
	* COD-79: Visualizar el promedio para la calificación de un profesor 
	* y promedio de un curso, a partir de los filtros.
	* Tarea Técnica: Tomar en cuenta las escogencias de todos los filtros.
	*
	* Cumplimiento: 10/10
*/


CREATE PROCEDURE [dbo].[PromedioProfesor]
	(	@UnidadesAcademicas FiltroUnidadesAcademicas READONLY,
		@CarrerasEnfasis FiltroCarrerasEnfasis READONLY,
		@Grupos FiltroGrupos READONLY,
		@CorreosProfesores FiltroProfesores READONLY,
		@promedio FLOAT OUTPUT,
		@cantidad INT OUTPUT,

		@nMalo INT OUTPUT,
		@nRegular INT OUTPUT,
		@nBueno INT OUTPUT
	)
	 
AS

BEGIN

DECLARE @min AS INT 
DECLARE @max AS INT
DECLARE @inc AS INT
DECLARE @opcion AS TINYINT
DECLARE @valor AS FLOAT = 0

	SET @promedio = -1;
	SET @nMalo = -1;
	SET @nRegular = -1;
	SET @nBueno = -1;

	SELECT @min = E.Minimo, @max = E.Maximo, @inc = E.Incremento
	FROM Escalar AS E 
	WHERE E.Codigo = 'INFPROF'

	DECLARE @Resultados TABLE (Opcion_seleccionada TINYINT);
	
	/*
	* Se recuperan todas las opciones escogidas 
	* de todas las preguntas de tipo 'INFPROF'
	* de todos los fomularios ya finalizados 
	* que existan dentro de los formularios 
	* recuperados por la función ObtenerFormulariosFiltros.
	*/

	INSERT INTO @Resultados
		SELECT  O.OpcionSeleccionada 
		FROM	Opciones_seleccionadas_respuesta_con_opciones AS O
				JOIN Respuestas_a_formulario AS R ON 
				O.FCodigo = R.FCodigo AND
				O.Correo = R.Correo AND
				O.CSigla = R.CSigla AND
				O.GNumero = R.GNumero AND
				O.GAnno = R.GAnno AND
				O.GSemestre = R.GSemestre AND
				O.Fecha = R.Fecha
		WHERE	O.PCodigo = 'INFPROF' AND 
				R.Finalizado = 1 AND
				EXISTS (SELECT *
						FROM ObtenerFormulariosFiltros(@UnidadesAcademicas, @CarrerasEnfasis, @Grupos, @CorreosProfesores)
						WHERE	FCodigo = O.FCodigo AND 
								CSigla = O.CSigla AND 
								GNumero = O.GNumero AND 
								GSemestre = O.GSemestre AND 
								GAnno = O.GAnno);

	/*
	* Se calcula el valor de las opciones seleccionadas 
	* con base en el mínimo, máximo e incremento 
	* dada una pregunta de tipo escalar.
	*/

	DECLARE C CURSOR FOR SELECT Opcion_seleccionada FROM @Resultados
	OPEN C
	FETCH NEXT FROM C INTO @opcion
	WHILE @@FETCH_STATUS = 0 
	BEGIN
		SET @valor = @valor + ((@opcion * @inc)+@min); 
		FETCH NEXT FROM C INTO @opcion
	END
	CLOSE C 
	DEALLOCATE C
	
	SELECT @cantidad = COUNT(*)
	FROM @Resultados

	IF (@cantidad != 0)
	BEGIN
		/*Se calcula el promedio con base en el valor obtenido y la cantidad de calificaciones*/
		SET @promedio = @valor / @cantidad;

		/*Cantidad de calificaciones entre 0-5*/
		SELECT @nMalo = COUNT(*)
		FROM @Resultados
		WHERE (0 <= Opcion_seleccionada AND
					Opcion_seleccionada <= 5)

		/*Cantidad de calificaciones entre 6-7*/
		SELECT @nRegular = COUNT(*)
		FROM @Resultados
		WHERE (5 < Opcion_seleccionada AND
					Opcion_seleccionada <= 7)

		/*Cantidad de calificaciones entre 8-10*/
		SELECT @nBueno = COUNT(*)
		FROM @Resultados
		WHERE (7 < Opcion_seleccionada AND
					Opcion_seleccionada <= 10)

	END

END