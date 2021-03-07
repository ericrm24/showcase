/*
Retorna una tabla con el código de carrera, nombre de carrera, código de énfasis, nombre de énfasis, de todos las carreras y énfasis que pertenecen a las unidades académicas dadas como parámetro.
Esta información se necesita para filtrar carreras y énfasis.
*/
CREATE FUNCTION ObtenerCarrerasEnfasisUA (@UnidadesAcademicas FiltroUnidadesAcademicas READONLY)
RETURNS @carrerasEnfasisUA TABLE
(
	CodCarrera VARCHAR(10),		/*Código de la carrera*/
	NomCarrera VARCHAR(50),		/*Nombre de la carrera*/
	CodEnfasis VARCHAR(10),		/*Código del énfasis*/
	NomEnfasis VARCHAR(50),		/*Nombre del énfasis*/
	PRIMARY KEY (CodCarrera, NomEnfasis)
)
AS
BEGIN

	/*Almacena resultados de forma temporal para poder hacer las uniones.*/
	DECLARE @carrerasEnfasisTemp TABLE
	(
		CodCarrera VARCHAR(10),		/*Código de la carrera*/
		NomCarrera VARCHAR(50),		/*Nombre de la carrera*/
		CodEnfasis VARCHAR(10),		/*Código del énfasis*/
		NomEnfasis VARCHAR(50),		/*Nombre del énfasis*/
		PRIMARY KEY (CodCarrera, NomEnfasis)
	);
	
	DECLARE @codigoUA VARCHAR(10);
	DECLARE UA CURSOR FOR SELECT CodigoUA FROM @UnidadesAcademicas;

	OPEN UA;
	FETCH NEXT FROM UA INTO @codigoUA;

	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si el código de la unidad académica no es nulo, verifica si es válido y recupera la información de las carreras y énfasis.*/
		IF (@codigoUA IS NOT NULL)
		BEGIN

			/*Si el código de la unidad académica es válido, recupera la información de las carreras y énfasis.*/
			IF (EXISTS (SELECT * FROM UnidadAcademica WHERE Codigo = @codigoUA))
			BEGIN

				INSERT INTO @carrerasEnfasisTemp
				SELECT	C.Codigo, C.Nombre, E.Codigo, E.Nombre
				FROM	Carrera AS C JOIN Enfasis AS E ON C.Codigo = E.CodCarrera
				WHERE	EXISTS	(
										SELECT *
										FROM UnidadAcademica AS U
										JOIN Inscrita_en AS I ON U.Codigo = I.CodUnidadAc
										WHERE U.Codigo = @codigoUA AND I.CodCarrera = C.Codigo /*Solo de carreras y énfasis de la unidad académica.*/
									)
				UNION
				SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
				FROM @carrerasEnfasisUA;

				/*Limpia las variables y actualiza carrerasEnfasisUA con el nuevo resultado.*/
				DELETE FROM @carrerasEnfasisUA;

				INSERT INTO @carrerasEnfasisUA
				SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
				FROM @carrerasEnfasisTemp;

				DELETE FROM @carrerasEnfasisTemp;

			END;

		END;

		FETCH NEXT FROM UA INTO @codigoUA;

	END;

	CLOSE UA;
	DEALLOCATE UA;

	RETURN;

END;