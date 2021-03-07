/*
Retorna una tabla con el código y nombre de las unidades académicas según los filtros de búsqueda.
Esta información se necesita para filtrar unidades académicas.
*/
CREATE FUNCTION ObtenerUAsFiltros (
@CarrerasEnfasis FiltroCarrerasEnfasis READONLY,
@Grupos FiltroGrupos READONLY,
@CorreosProfesores FiltroProfesores READONLY
)
RETURNS @uasFiltros TABLE
(
	CodigoUA VARCHAR(10)	PRIMARY KEY,	/*Código de la unidad académica*/
	NombreUA VARCHAR(50)					/*Nombre de la unidad académica*/
)
AS
BEGIN

	/*Almacena resultados de forma temporal para poder hacer las intersecciones.*/
	DECLARE @uasTemp TABLE
	(
		CodigoUA VARCHAR(10)	PRIMARY KEY,	/*Código de la unidad académica*/
		NombreUA VARCHAR(50)					/*Nombre de la unidad académica*/
	);

	INSERT INTO @uasFiltros
	SELECT	U.Codigo, U.Nombre
	FROM	UnidadAcademica AS U;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @CarrerasEnfasis WHERE CodigoCarrera IS NOT NULL AND CodigoEnfasis IS NOT NULL))
	BEGIN

		/*Llena uasTemp con la intersección de los resultados del nuevo filtro y los resultados de uasFiltros.*/
		INSERT INTO @uasTemp
		SELECT CodigoUA, NombreUA
		FROM ObtenerUAsCarreraEnfasis(@CarrerasEnfasis)
		INTERSECT
		SELECT CodigoUA, NombreUA
		FROM @uasFiltros;

		/*Limpia las variables y actualiza uasFiltros con el nuevo resultado.*/
		DELETE FROM @uasFiltros;

		INSERT INTO @uasFiltros
		SELECT CodigoUA, NombreUA
		FROM @uasTemp;

		DELETE FROM @uasTemp;

	END;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @Grupos WHERE SiglaCurso IS NOT NULL AND NumeroGrupo IS NOT NULL AND Semestre IS NOT NULL AND Anno IS NOT NULL))
	BEGIN

		/*Llena uasTemp con la intersección de los resultados del nuevo filtro y los resultados de uasFiltros.*/
		INSERT INTO @uasTemp
		SELECT CodigoUA, NombreUA
		FROM ObtenerUAsGrupo(@Grupos)
		INTERSECT
		SELECT CodigoUA, NombreUA
		FROM @uasFiltros;

		/*Limpia las variables y actualiza uasFiltros con el nuevo resultado.*/
		DELETE FROM @uasFiltros;

		INSERT INTO @uasFiltros
		SELECT CodigoUA, NombreUA
		FROM @uasTemp;

		DELETE FROM @uasTemp;

	END;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @CorreosProfesores WHERE CorreoProfesor IS NOT NULL))
	BEGIN

		/*Llena uasTemp con la intersección de los resultados del nuevo filtro y los resultados de uasFiltros.*/
		INSERT INTO @uasTemp
		SELECT CodigoUA, NombreUA
		FROM ObtenerUAsProfesor(@CorreosProfesores)
		INTERSECT
		SELECT CodigoUA, NombreUA
		FROM @uasFiltros;

		/*Limpia las variables y actualiza uasFiltros con el nuevo resultado.*/
		DELETE FROM @uasFiltros;

		INSERT INTO @uasFiltros
		SELECT CodigoUA, NombreUA
		FROM @uasTemp;

		DELETE FROM @uasTemp;

	END;

	RETURN;

END;
