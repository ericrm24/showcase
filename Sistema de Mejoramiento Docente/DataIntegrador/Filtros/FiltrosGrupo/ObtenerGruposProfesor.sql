/*
Retorna una tabla con la sigla del curso, nombre del curso, número de grupo, semestre, año, de todos los grupos de cursos que imparten los profesores dados como parámetro.
Esta información se necesita para filtrar grupos.
*/
CREATE FUNCTION ObtenerGruposProfesor (@CorreosProfesores FiltroProfesores READONLY)
RETURNS @gruposProfesor TABLE
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

	DECLARE @correoProfesor VARCHAR(50);
	DECLARE P CURSOR FOR SELECT CorreoProfesor FROM @CorreosProfesores;

	OPEN P;
	FETCH NEXT FROM P INTO @correoProfesor;
	
	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si el correo del profesor no es nulo, verifica si es válido y recupera la información de los grupos.*/
		IF (@correoProfesor IS NOT NULL)
		BEGIN

			/*Si el correo del profesor es válido, recupera la información de los grupos.*/
			IF (EXISTS (SELECT * FROM Profesor WHERE Correo = @correoProfesor))
			BEGIN

				INSERT INTO @gruposTemp
				SELECT	C.Sigla, C.Nombre, G.NumGrupo, G.Semestre, G.Anno
				FROM	Curso AS C JOIN Grupo AS G ON C.Sigla = G.SiglaCurso
				WHERE	EXISTS	(
										SELECT *
										FROM Imparte AS I
										WHERE I.CorreoProfesor = @correoProfesor /*Solo de grupos que imparte el profesor con el correo parámetro.*/
										AND I.SiglaCurso = G.SiglaCurso
										AND I.NumGrupo = G.NumGrupo
										AND I.Semestre = G.Semestre
										AND I.Anno = G.Anno
									)
				UNION
				SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
				FROM @gruposProfesor;

				/*Limpia las variables y actualiza gruposProfesor con el nuevo resultado.*/
				DELETE FROM @gruposProfesor;

				INSERT INTO @gruposProfesor
				SELECT SiglaCurso, NombreCurso, NumGrupo, Semestre, Anno
				FROM @gruposTemp;

				DELETE FROM @gruposTemp;

			END;

		END;

		FETCH NEXT FROM P INTO @correoProfesor;

	END;

	CLOSE P;
	DEALLOCATE P;

	RETURN;

END;