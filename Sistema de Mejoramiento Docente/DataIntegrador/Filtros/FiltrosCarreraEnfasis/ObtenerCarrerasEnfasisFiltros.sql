/*
Retorna una tabla con el código de carrera, nombre de carrera, código de énfasis, nombre de énfasis, de todos las carreras y énfasis según los filtros de búsqueda.
Esta información se necesita para filtrar carreras y énfasis.
*/
CREATE FUNCTION ObtenerCarrerasEnfasisFiltros (
@UnidadesAcademicas FiltroUnidadesAcademicas READONLY,
@Grupos FiltroGrupos READONLY,
@CorreosProfesores FiltroProfesores READONLY
)
RETURNS @carrerasEnfasisFiltros TABLE
(
	CodCarrera VARCHAR(10),		/*Código de la carrera*/
	NomCarrera VARCHAR(50),		/*Nombre de la carrera*/
	CodEnfasis VARCHAR(10),		/*Código del énfasis*/
	NomEnfasis VARCHAR(50),		/*Nombre del énfasis*/
	PRIMARY KEY (CodCarrera, NomEnfasis)
)
AS
BEGIN

	/*Almacena resultados de forma temporal para poder hacer las intersecciones.*/
	DECLARE @carrerasEnfasisTemp TABLE
	(
		CodCarrera VARCHAR(10),		/*Código de la carrera*/
		NomCarrera VARCHAR(50),		/*Nombre de la carrera*/
		CodEnfasis VARCHAR(10),		/*Código del énfasis*/
		NomEnfasis VARCHAR(50),		/*Nombre del énfasis*/
		PRIMARY KEY (CodCarrera, NomEnfasis)
	);

	INSERT INTO @carrerasEnfasisFiltros
	SELECT C.Codigo, C.Nombre, E.Codigo, E.Nombre
	FROM Carrera AS C JOIN Enfasis AS E ON C.Codigo = E.CodCarrera;
	
	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @UnidadesAcademicas WHERE CodigoUA IS NOT NULL))
	BEGIN

		/*Llena carrerasEnfasisTemp con la intersección de los resultados del nuevo filtro y los resultados de carrerasEnfasisFiltros.*/
		INSERT INTO @carrerasEnfasisTemp
		SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
		FROM ObtenerCarrerasEnfasisUA(@UnidadesAcademicas)
		INTERSECT
		SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
		FROM @carrerasEnfasisFiltros;

		/*Limpia las variables y actualiza carrerasEnfasisFiltros con el nuevo resultado.*/
		DELETE FROM @carrerasEnfasisFiltros;

		INSERT INTO @carrerasEnfasisFiltros
		SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
		FROM @carrerasEnfasisTemp;

		DELETE FROM @carrerasEnfasisTemp;

	END;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @Grupos WHERE SiglaCurso IS NOT NULL AND NumeroGrupo IS NOT NULL AND Semestre IS NOT NULL AND Anno IS NOT NULL))
	BEGIN

		/*Llena carrerasEnfasisTemp con la intersección de los resultados del nuevo filtro y los resultados de carrerasEnfasisFiltros.*/
		INSERT INTO @carrerasEnfasisTemp
		SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
		FROM ObtenerCarrerasEnfasisGrupo(@Grupos)
		INTERSECT
		SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
		FROM @carrerasEnfasisFiltros;

		/*Limpia las variables y actualiza carrerasEnfasisFiltros con el nuevo resultado.*/
		DELETE FROM @carrerasEnfasisFiltros;

		INSERT INTO @carrerasEnfasisFiltros
		SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
		FROM @carrerasEnfasisTemp;

		DELETE FROM @carrerasEnfasisTemp;

	END;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @CorreosProfesores WHERE CorreoProfesor IS NOT NULL))
	BEGIN

		/*Llena carrerasEnfasisTemp con la intersección de los resultados del nuevo filtro y los resultados de carrerasEnfasisFiltros.*/
		INSERT INTO @carrerasEnfasisTemp
		SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
		FROM ObtenerCarrerasEnfasisProfesor(@CorreosProfesores)
		INTERSECT
		SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
		FROM @carrerasEnfasisFiltros;

		/*Limpia las variables y actualiza carrerasEnfasisFiltros con el nuevo resultado.*/
		DELETE FROM @carrerasEnfasisFiltros;

		INSERT INTO @carrerasEnfasisFiltros
		SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
		FROM @carrerasEnfasisTemp;

		DELETE FROM @carrerasEnfasisTemp;

	END;

	RETURN;

END;
