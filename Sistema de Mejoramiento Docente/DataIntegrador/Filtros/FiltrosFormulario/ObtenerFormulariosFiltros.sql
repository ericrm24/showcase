/*
Retorna una tabla con el código de formulario, sigla del curso, número de grupo, semestre, año, fecha de inicio, fecha de finalización de todos los formularios según los filtros de búsqueda.
Esta información se necesita para filtrar respuestas a formulario.
*/
CREATE FUNCTION ObtenerFormulariosFiltros (
@UnidadesAcademicas FiltroUnidadesAcademicas READONLY,
@CarrerasEnfasis FiltroCarrerasEnfasis READONLY,
@Grupos FiltroGrupos READONLY,
@CorreosProfesores FiltroProfesores READONLY
)
RETURNS @formulariosFiltros TABLE
(
	FCodigo VARCHAR(8),		/*Código del formulario.*/
	FNombre NVARCHAR(250),	/*Nombre del formulario*/
	CSigla VARCHAR(10),		/*Sigla del curso.*/
	GNumero TINYINT,		/*Número de grupo.*/
	GSemestre TINYINT,		/*Número de semestre.*/
	GAnno INT,				/*Año.*/
	FechaInicio DATE,		/*Fecha de inicio del periodo de llenado el formulario.*/
	FechaFin DATE,			/*Fecha de finalización del periodo de llenado del formulario.*/
	PRIMARY KEY (FCodigo, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin)
)
AS
BEGIN

	/*Almacena resultados de forma temporal para poder hacer las intersecciones.*/
	DECLARE @formulariosTemp TABLE
	(
		FCodigo VARCHAR(8),		/*Código del formulario.*/
		FNombre NVARCHAR(250),	/*Nombre del formulario*/
		CSigla VARCHAR(10),		/*Sigla del curso.*/
		GNumero TINYINT,		/*Número de grupo.*/
		GSemestre TINYINT,		/*Número de semestre.*/
		GAnno INT,				/*Año.*/
		FechaInicio DATE,		/*Fecha de inicio del periodo de llenado el formulario.*/
		FechaFin DATE,			/*Fecha de finalización del periodo de llenado del formulario.*/
		PRIMARY KEY (FCodigo, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin)
	);

	INSERT INTO @formulariosFiltros
	SELECT	PAP.FCodigo, F.Nombre, PAP.CSigla, PAP.GNumero, PAP.GSemestre, PAP.GAnno, PAP.FechaInicio, PAP.FechaFin
	FROM	Periodo_activa_por AS PAP JOIN Formulario AS F ON PAP.FCodigo = F.Codigo;
	
	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @UnidadesAcademicas WHERE CodigoUA IS NOT NULL))
	BEGIN

		/*Llena formulariosTemp con la intersección de los resultados del nuevo filtro y los resultados de formulariosFiltros.*/
		INSERT INTO @formulariosTemp
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM ObtenerFormulariosUA(@UnidadesAcademicas)
		INTERSECT
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM @formulariosFiltros;

		/*Limpia las variables y actualiza formulariosFiltros con el nuevo resultado.*/
		DELETE FROM @formulariosFiltros;

		INSERT INTO @formulariosFiltros
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM @formulariosTemp;

		DELETE FROM @formulariosTemp;

	END;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @CarrerasEnfasis WHERE CodigoCarrera IS NOT NULL AND CodigoEnfasis IS NOT NULL))
	BEGIN

		/*Llena formulariosTemp con la intersección de los resultados del nuevo filtro y los resultados de formulariosFiltros.*/
		INSERT INTO @formulariosTemp
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM ObtenerFormulariosCarreraEnfasis(@CarrerasEnfasis)
		INTERSECT
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM @formulariosFiltros;

		/*Limpia las variables y actualiza formulariosFiltros con el nuevo resultado.*/
		DELETE FROM @formulariosFiltros;

		INSERT INTO @formulariosFiltros
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM @formulariosTemp;

		DELETE FROM @formulariosTemp;

	END;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @Grupos WHERE SiglaCurso IS NOT NULL AND NumeroGrupo IS NOT NULL AND Semestre IS NOT NULL AND Anno IS NOT NULL))
	BEGIN

		/*Llena formulariosTemp con la intersección de los resultados del nuevo filtro y los resultados de formulariosFiltros.*/
		INSERT INTO @formulariosTemp
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM ObtenerFormulariosGrupo(@Grupos)
		INTERSECT
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM @formulariosFiltros;

		/*Limpia las variables y actualiza formulariosFiltros con el nuevo resultado.*/
		DELETE FROM @formulariosFiltros;

		INSERT INTO @formulariosFiltros
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM @formulariosTemp;

		DELETE FROM @formulariosTemp;

	END;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @CorreosProfesores WHERE CorreoProfesor IS NOT NULL))
	BEGIN

		/*Llena formulariosTemp con la intersección de los resultados del nuevo filtro y los resultados de formulariosFiltros.*/
		INSERT INTO @formulariosTemp
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM ObtenerFormulariosProfesor(@CorreosProfesores)
		INTERSECT
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM @formulariosFiltros;

		/*Limpia las variables y actualiza formulariosFiltros con el nuevo resultado.*/
		DELETE FROM @formulariosFiltros;

		INSERT INTO @formulariosFiltros
		SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
		FROM @formulariosTemp;

		DELETE FROM @formulariosTemp;

	END;

	RETURN;

END;
