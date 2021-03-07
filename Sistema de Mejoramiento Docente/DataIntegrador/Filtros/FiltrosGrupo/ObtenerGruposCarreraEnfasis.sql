/*
Retorna una tabla con la sigla del curso, nombre del curso, número de grupo, semestre, año, de todos los grupos de cursos que pertenecen a las carreras y énfasis dadas como parámetro.
Esta información se necesita para filtrar grupos.
*/
CREATE FUNCTION ObtenerGruposCarreraEnfasis (@CarrerasEnfasis FiltroCarrerasEnfasis READONLY)
RETURNS @gruposCarreraEnfasis TABLE
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

	/*Almacena resultados de forma temporal para poder hacer las uniones.*/
	DECLARE @gruposTemp TABLE

	(
		SiglaCurso VARCHAR(10),		/*Sigla del curso*/
		NombreCurso VARCHAR(50),	/*Nombre del curso*/
		NumGrupo TINYINT,			/*Número de grupo*/
		Semestre TINYINT,			/*Semestre*/
		Anno INT,					/*Anno*/
		PRIMARY KEY (SiglaCurso, NumGrupo, Semestre, Anno)
	);

	DECLARE @codigoCarrera VARCHAR(10);
	DECLARE @codigoEnfasis VARCHAR(10);
	DECLARE CE CURSOR FOR SELECT CodigoCarrera, CodigoEnfasis FROM @CarrerasEnfasis;

	OPEN CE;
	FETCH NEXT FROM CE INTO @codigoCarrera, @codigoEnfasis;
	
	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si el código de la carrera y énfasis no son nulos, verifica si son válidos y recupera la información de los grupos.*/
		IF (@codigoCarrera IS NOT NULL AND @codigoEnfasis IS NOT NULL)
		BEGIN

			/*Si el código de la carrera y énfasis son válidos, recupera la información de los grupos.*/
			IF (EXISTS (SELECT * FROM Enfasis WHERE CodCarrera = @codigoCarrera AND Codigo = @codigoEnfasis))
			BEGIN

				INSERT INTO @gruposTemp
				SELECT	C.Sigla, C.Nombre, G.NumGrupo, G.Semestre, G.Anno
				FROM	Curso AS C JOIN Grupo AS G ON C.Sigla = G.SiglaCurso
				WHERE	EXISTS	(
										SELECT *
										FROM UnidadAcademica AS U
										JOIN Inscrita_en AS I ON U.Codigo = I.CodUnidadAc
										JOIN Pertenece_a AS PE ON I.CodCarrera = PE.CodCarrera
										WHERE PE.CodCarrera = @codigoCarrera AND PE.CodEnfasis = @codigoEnfasis AND PE.SiglaCurso = C.Sigla /*Solo de grupos de los cursos que contiene el énfasis.*/
									)
				UNION
				SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
				FROM @gruposCarreraEnfasis;

				/*Limpia las variables y actualiza gruposCarreraEnfasis con el nuevo resultado.*/
				DELETE FROM @gruposCarreraEnfasis;

				INSERT INTO @gruposCarreraEnfasis
				SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
				FROM @gruposTemp;

				DELETE FROM @gruposTemp;

			END;

		END;

		FETCH NEXT FROM CE INTO @codigoCarrera, @codigoEnfasis;

	END;

	CLOSE CE;
	DEALLOCATE CE;

	RETURN;

END;
