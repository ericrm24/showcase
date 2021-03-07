/*
Retorna una tabla con el código y nombre de las unidades académicas a las que pertenecen cursos de grupos que los profesores dados como parámetro imparten.
Esta información se necesita para filtrar unidades académicas.
*/
CREATE FUNCTION ObtenerUAsProfesor (@CorreosProfesores FiltroProfesores READONLY)
RETURNS @uasProfesor TABLE
(
	CodigoUA VARCHAR(10)	PRIMARY KEY,	/*Código de la unidad académica*/
	NombreUA VARCHAR(50)					/*Nombre de la unidad académica*/
)
AS
BEGIN

	/*Almacena resultados de forma temporal para poder hacer las uniones.*/
	DECLARE @uasTemp TABLE
	(
		CodigoUA VARCHAR(10)	PRIMARY KEY,	/*Código de la unidad académica*/
		NombreUA VARCHAR(50)					/*Nombre de la unidad académica*/
	);

	DECLARE @correoProfesor VARCHAR(50);
	DECLARE P CURSOR FOR SELECT CorreoProfesor FROM @CorreosProfesores;

	OPEN P;
	FETCH NEXT FROM P INTO @correoProfesor;
	
	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si el correo del profesor no es nulo, verifica si es válido y recupera la información de las unidades académicas.*/
		IF (@correoProfesor IS NOT NULL)
		BEGIN

			/*Si el correo del profesor es válido, recupera la información de las unidades académicas.*/
			IF (EXISTS (SELECT * FROM Profesor WHERE Correo = @correoProfesor))
			BEGIN

				INSERT INTO @uasTemp
				SELECT	U.Codigo, U.Nombre
				FROM	UnidadAcademica AS U
				WHERE	EXISTS	(
									SELECT *
									FROM Inscrita_en AS I
									JOIN Pertenece_a AS PE ON I.CodCarrera = PE.CodCarrera 
									JOIN Imparte AS IM ON PE.SiglaCurso = IM.SiglaCurso
									WHERE I.CodUnidadAc = U.Codigo AND IM.CorreoProfesor = @correoProfesor /*Solo unidades académicas con cursos de grupos que el profesor dado como parámetro imparte.*/
								)
				UNION
				SELECT CodigoUA, NombreUA
				FROM @uasProfesor;

				/*Limpia las variables y actualiza uasProfesor con el nuevo resultado.*/
				DELETE FROM @uasProfesor;

				INSERT INTO @uasProfesor
				SELECT CodigoUA, NombreUA
				FROM @uasTemp;

				DELETE FROM @uasTemp;

			END;

		END;

		FETCH NEXT FROM P INTO @correoProfesor;

	END;

	CLOSE P;
	DEALLOCATE P;

	RETURN;

END;