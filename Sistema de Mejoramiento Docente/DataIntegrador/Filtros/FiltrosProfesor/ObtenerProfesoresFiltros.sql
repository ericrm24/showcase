/*
Retorna una tabla con el correo, primer y segundo nombre, primer y segundo apellido de todos los profesores según los filtros de búsqueda.
Esta información se necesita para filtrar profesores.
*/
CREATE FUNCTION ObtenerProfesoresFiltros (
@UnidadesAcademicas FiltroUnidadesAcademicas READONLY,
@CarrerasEnfasis FiltroCarrerasEnfasis READONLY,
@Grupos FiltroGrupos READONLY
)
RETURNS @profesoresFiltros TABLE
(
	Correo		VARCHAR(50) PRIMARY KEY,	/*Correo del profesor*/
	Nombre1		VARCHAR(15) NOT NULL,		/*Primer nonmbre del profesor*/
    Nombre2		VARCHAR(15) NULL,			/*Segundo nonmbre del profesor*/
    Apellido1	VARCHAR(15) NOT NULL,		/*Primer apellido del profesor*/
    Apellido2	VARCHAR(15) NULL			/*Segundo apellido del profesor*/
)
AS
BEGIN

	/*Almacena resultados de forma temporal para poder hacer las intersecciones.*/
	DECLARE @profesoresTemp TABLE
	(
		Correo		VARCHAR(50) PRIMARY KEY,	/*Correo del profesor*/
		Nombre1		VARCHAR(15) NOT NULL,		/*Primer nonmbre del profesor*/
		Nombre2		VARCHAR(15) NULL,			/*Segundo nonmbre del profesor*/
		Apellido1	VARCHAR(15) NOT NULL,		/*Primer apellido del profesor*/
		Apellido2	VARCHAR(15) NULL			/*Segundo apellido del profesor*/
	);

	INSERT INTO @profesoresFiltros
	SELECT	DISTINCT P.Correo, P.Nombre1, P.Nombre2, P.Apellido1, P.Apellido2
	FROM	Persona AS P JOIN Imparte AS Im ON P.Correo = Im.CorreoProfesor;
	
	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @UnidadesAcademicas WHERE CodigoUA IS NOT NULL))
	BEGIN

		/*Llena profesoresTemp con la intersección de los resultados del nuevo filtro y los resultados de profesoresFiltros.*/
		INSERT INTO @profesoresTemp
		SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
		FROM ObtenerProfesoresUA(@UnidadesAcademicas)
		INTERSECT
		SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
		FROM @profesoresFiltros;

		/*Limpia las variables y actualiza profesoresFiltros con el nuevo resultado.*/
		DELETE FROM @profesoresFiltros;

		INSERT INTO @profesoresFiltros
		SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
		FROM @profesoresTemp;

		DELETE FROM @profesoresTemp;

	END;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @CarrerasEnfasis WHERE CodigoCarrera IS NOT NULL AND CodigoEnfasis IS NOT NULL))
	BEGIN

		/*Llena profesoresTemp con la intersección de los resultados del nuevo filtro y los resultados de profesoresFiltros.*/
		INSERT INTO @profesoresTemp
		SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
		FROM ObtenerProfesoresCarreraEnfasis(@CarrerasEnfasis)
		INTERSECT
		SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
		FROM @profesoresFiltros;

		/*Limpia las variables y actualiza profesoresFiltros con el nuevo resultado.*/
		DELETE FROM @profesoresFiltros;

		INSERT INTO @profesoresFiltros
		SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
		FROM @profesoresTemp;

		DELETE FROM @profesoresTemp;

	END;

	/*Si existe algún parámetro que no sea nulo, se toma en cuenta para el filtro.*/
	IF (EXISTS (SELECT * FROM @Grupos WHERE SiglaCurso IS NOT NULL AND NumeroGrupo IS NOT NULL AND Semestre IS NOT NULL AND Anno IS NOT NULL))
	BEGIN

		/*Llena profesoresTemp con la intersección de los resultados del nuevo filtro y los resultados de profesoresFiltros.*/
		INSERT INTO @profesoresTemp
		SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
		FROM ObtenerProfesoresGrupo(@Grupos)
		INTERSECT
		SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
		FROM @profesoresFiltros;

		/*Limpia las variables y actualiza profesoresFiltros con el nuevo resultado.*/
		DELETE FROM @profesoresFiltros;

		INSERT INTO @profesoresFiltros
		SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
		FROM @profesoresTemp;

		DELETE FROM @profesoresTemp;

	END;

	RETURN;

END;
