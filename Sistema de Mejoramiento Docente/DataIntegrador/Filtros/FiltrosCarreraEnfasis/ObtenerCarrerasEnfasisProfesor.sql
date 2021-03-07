/*
Retorna una tabla con el código de carrera, nombre de carrera, código de énfasis, nombre de énfasis, de todos las carreras y énfasis a los que pertenecen cursos que imparten los profesores dados como parámetro.
Esta información se necesita para filtrar carreras y énfasis.
*/
CREATE FUNCTION ObtenerCarrerasEnfasisProfesor (@CorreosProfesores FiltroProfesores READONLY)
RETURNS @carrerasEnfasisProfesor TABLE
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

	DECLARE @correoProfesor VARCHAR(50);
	DECLARE P CURSOR FOR SELECT CorreoProfesor FROM @CorreosProfesores;

	OPEN P;
	FETCH NEXT FROM P INTO @correoProfesor;
	
	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si el correo del profesor no es nulo, verifica si es válido y recupera la información de las carreras y énfasis.*/
		IF (@correoProfesor IS NOT NULL)
		BEGIN

			/*Si el correo del profesor es válido, recupera la información de las carreras y énfasiss.*/
			IF (EXISTS (SELECT * FROM Profesor WHERE Correo = @correoProfesor))
			BEGIN

				INSERT INTO @carrerasEnfasisTemp
				SELECT	C.Codigo, C.Nombre, E.Codigo, E.Nombre
				FROM	Carrera AS C JOIN Enfasis AS E ON C.Codigo = E.CodCarrera
				WHERE	EXISTS	(
									SELECT *
									FROM Pertenece_a AS PE JOIN Imparte AS I ON PE.SiglaCurso = I.SiglaCurso
									WHERE PE.CodCarrera = C.Codigo AND PE.CodEnfasis = E.Codigo AND I.CorreoProfesor = @correoProfesor /*Solo carreras y énfasis con cursos de grupos que el profesor dado como parámetro imparte.*/
								)
				UNION
				SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
				FROM @carrerasEnfasisProfesor;

				/*Limpia las variables y actualiza carrerasEnfasisProfesor con el nuevo resultado.*/
				DELETE FROM @carrerasEnfasisProfesor;

				INSERT INTO @carrerasEnfasisProfesor
				SELECT CodCarrera, NomCarrera, CodEnfasis, NomEnfasis
				FROM @carrerasEnfasisTemp;

				DELETE FROM @carrerasEnfasisTemp;

			END;

		END;

		FETCH NEXT FROM P INTO @correoProfesor;

	END;

	CLOSE P;
	DEALLOCATE P;

	RETURN;

END;