/*
Retorna una tabla con la sigla del curso, nombre del curso, número de grupo, semestre, año, de todos los grupos de cursos que pertenecen a las unidades académicas dadas como parámetro.
Esta información se necesita para filtrar grupos.
*/
CREATE FUNCTION ObtenerGruposUA (@UnidadesAcademicas FiltroUnidadesAcademicas READONLY)
RETURNS @gruposUA TABLE
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
	
	DECLARE @codigoUA VARCHAR(10);
	DECLARE UA CURSOR FOR SELECT CodigoUA FROM @UnidadesAcademicas;

	OPEN UA;
	FETCH NEXT FROM UA INTO @codigoUA;

	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si el código de la unidad académica no es nulo, verifica si es válido y recupera la información de los grupos.*/
		IF (@codigoUA IS NOT NULL)
		BEGIN

			/*Si el código de la unidad académica es válido, recupera la información de los grupos.*/
			IF (EXISTS (SELECT * FROM UnidadAcademica WHERE Codigo = @codigoUA))
			BEGIN

				INSERT INTO @gruposTemp
				SELECT	C.Sigla, C.Nombre, G.NumGrupo, G.Semestre, G.Anno
				FROM	Curso AS C JOIN Grupo AS G ON C.Sigla = G.SiglaCurso
				WHERE	EXISTS	(
										SELECT *
										FROM UnidadAcademica AS U
										JOIN Inscrita_en AS I ON U.Codigo = I.CodUnidadAc
										JOIN Pertenece_a AS PE ON I.CodCarrera = PE.CodCarrera
										WHERE U.Codigo = @codigoUA AND PE.SiglaCurso = C.Sigla /*Solo grupos de los cursos que contiene la unidad académica.*/
									)
				UNION
				SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
				FROM @gruposUA;

				/*Limpia las variables y actualiza gruposUA con el nuevo resultado.*/
				DELETE FROM @gruposUA;

				INSERT INTO @gruposUA
				SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
				FROM @gruposTemp;

				DELETE FROM @gruposTemp;

			END;

		END;

		FETCH NEXT FROM UA INTO @codigoUA;

	END;

	CLOSE UA;
	DEALLOCATE UA;

	RETURN;

END;