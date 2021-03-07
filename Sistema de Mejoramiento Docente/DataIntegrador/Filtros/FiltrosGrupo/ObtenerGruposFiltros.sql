/*
Retorna una tabla con el código de formulario, sigla del curso, número de grupo, semestre, año, fecha de inicio, fecha de finalización de todos los formularios según los filtros de búsqueda.
Esta información se necesita para filtrar respuestas a formulario.
*/
CREATE FUNCTION ObtenerGruposFiltros (
@UnidadesAcademicas FiltroUnidadesAcademicas READONLY,
@CarrerasEnfasis FiltroCarrerasEnfasis READONLY,
@CorreosProfesores FiltroProfesores READONLY
)
RETURNS @gruposFiltros TABLE
(
	SiglaCurso VARCHAR(10),		/*Sigla del curso*/
	NombreCurso VARCHAR(50),	/*Nombre del curso*/
	NumGrupo TINYINT,			/*Número de grupo*/
	Semestre TINYINT,			/*Semestre*/
	Anno INT,					/*Anno*/
	PRIMARY KEY (SiglaCurso, NumGrupo, Semestre, Anno)
)
AS
BEGIN

	/*Almacena resultados de forma temporal para poder hacer las intersecciones.*/
	DECLARE @gruposTemp TABLE
	(
		SiglaCurso VARCHAR(10),		/*Sigla del curso*/
		NombreCurso VARCHAR(50),	/*Nombre del curso*/
		NumGrupo TINYINT,			/*Número de grupo*/
		Semestre TINYINT,			/*Semestre*/
		Anno INT,					/*Anno*/
		PRIMARY KEY (SiglaCurso, NumGrupo, Semestre, Anno)
	);

	INSERT INTO @gruposFiltros
	SELECT	C.Sigla, C.Nombre, G.NumGrupo, G.Semestre, G.Anno
	FROM	Curso AS C JOIN Grupo AS G ON C.Sigla = G.SiglaCurso;
	
	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @UnidadesAcademicas WHERE CodigoUA IS NOT NULL))
	BEGIN

		/*Llena gruposTemp con la intersección de los resultados del nuevo filtro y los resultados de gruposFiltros.*/
		INSERT INTO @gruposTemp
		SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
		FROM ObtenerGruposUA(@UnidadesAcademicas)
		INTERSECT
		SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
		FROM @gruposFiltros;

		/*Limpia las variables y actualiza gruposFiltros con el nuevo resultado.*/
		DELETE FROM @gruposFiltros;

		INSERT INTO @gruposFiltros
		SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
		FROM @gruposTemp;

		DELETE FROM @gruposTemp;

	END;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @CarrerasEnfasis WHERE CodigoCarrera IS NOT NULL AND CodigoEnfasis IS NOT NULL))
	BEGIN

		/*Llena gruposTemp con la intersección de los resultados del nuevo filtro y los resultados de gruposFiltros.*/
		INSERT INTO @gruposTemp
		SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
		FROM ObtenerGruposCarreraEnfasis(@CarrerasEnfasis)
		INTERSECT
		SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
		FROM @gruposFiltros;

		/*Limpia las variables y actualiza gruposFiltros con el nuevo resultado.*/
		DELETE FROM @gruposFiltros;

		INSERT INTO @gruposFiltros
		SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
		FROM @gruposTemp;

		DELETE FROM @gruposTemp;

	END;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @CorreosProfesores WHERE CorreoProfesor IS NOT NULL))
	BEGIN

		/*Llena gruposTemp con la intersección de los resultados del nuevo filtro y los resultados de gruposFiltros.*/
		INSERT INTO @gruposTemp
		SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
		FROM ObtenerGruposProfesor(@CorreosProfesores)
		INTERSECT
		SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
		FROM @gruposFiltros;

		/*Limpia las variables y actualiza gruposFiltros con el nuevo resultado.*/
		DELETE FROM @gruposFiltros;

		INSERT INTO @gruposFiltros
		SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
		FROM @gruposTemp;

		DELETE FROM @gruposTemp;

	END;

	RETURN;

END;
