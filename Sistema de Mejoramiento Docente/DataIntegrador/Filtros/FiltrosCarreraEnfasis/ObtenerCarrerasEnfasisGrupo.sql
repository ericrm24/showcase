/*
Retorna una tabla con el código de carrera, nombre de carrera, código de énfasis, nombre de énfasis, de todos las carreras y énfasis que tienen los cursos de los grupos dados como parámetro.
Esta información se necesita para filtrar carreras y énfasis.
*/
CREATE FUNCTION ObtenerCarrerasEnfasisGrupo (@Grupos FiltroGrupos READONLY)
RETURNS @carrerasEnfasisGrupo TABLE
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

	DECLARE @siglaCurso VARCHAR(10);
	DECLARE @numeroGrupo TINYINT;
	DECLARE @semestre TINYINT;
	DECLARE @anno INT;
	DECLARE G CURSOR FOR SELECT SiglaCurso, NumeroGrupo, Semestre, Anno FROM @Grupos;

	OPEN G;
	FETCH NEXT FROM G INTO @siglaCurso, @numeroGrupo, @semestre, @anno;
	
	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si los parámetros del grupo no son nulos, verifica si son válidos y recupera la información de las carreras y énfasis.*/
		IF (@siglaCurso IS NOT NULL AND @numeroGrupo IS NOT NULL AND @semestre IS NOT NULL AND @anno IS NOT NULL)
		BEGIN

			/*Si los parámetros del grupo son válidos, recupera la información de las carreras y énfasis.*/
			IF (EXISTS (SELECT * FROM Grupo WHERE SiglaCurso = @siglaCurso AND NumGrupo = @numeroGrupo AND Semestre = @semestre AND Anno = @anno))
			BEGIN

				INSERT INTO @carrerasEnfasisTemp
				SELECT	C.Codigo, C.Nombre, E.Codigo, E.Nombre
				FROM	Carrera AS C JOIN Enfasis AS E ON C.Codigo = E.CodCarrera
				WHERE	EXISTS (
									SELECT *
									FROM Pertenece_a AS PE
									WHERE PE.CodCarrera = C.Codigo AND PE.CodEnfasis = E.Codigo AND PE.SiglaCurso = @siglaCurso /*Solo de carreas y énfasis a los que pertenece el curso del grupo dado como parámetro.*/
								)
				UNION
				SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
				FROM @carrerasEnfasisGrupo;

				/*Limpia las variables y actualiza carrerasEnfasisGrupo con el nuevo resultado.*/
				DELETE FROM @carrerasEnfasisGrupo;

				INSERT INTO @carrerasEnfasisGrupo
				SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
				FROM @carrerasEnfasisTemp;

				DELETE FROM @carrerasEnfasisTemp;

			END;

		END;

		FETCH NEXT FROM G INTO @siglaCurso, @numeroGrupo, @semestre, @anno;

	END;

	CLOSE G;
	DEALLOCATE G;

	RETURN;

END;