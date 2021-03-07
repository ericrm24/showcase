/*
Retorna una tabla con el código y nombre de las unidades académicas a las que pertenecen los cursos de los grupos dados como parámetro.
Esta información se necesita para filtrar unidades académicas.
*/
CREATE FUNCTION ObtenerUAsGrupo (@Grupos FiltroGrupos READONLY)
RETURNS @uasGrupo TABLE
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

	DECLARE @siglaCurso VARCHAR(10);
	DECLARE @numeroGrupo TINYINT;
	DECLARE @semestre TINYINT;
	DECLARE @anno INT;
	DECLARE G CURSOR FOR SELECT SiglaCurso, NumeroGrupo, Semestre, Anno FROM @Grupos;

	OPEN G;
	FETCH NEXT FROM G INTO @siglaCurso, @numeroGrupo, @semestre, @anno;
	
	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si los parámetros del grupo no son nulos, verifica si son válidos y recupera la información de las unidades académicas.*/
		IF (@siglaCurso IS NOT NULL AND @numeroGrupo IS NOT NULL AND @semestre IS NOT NULL AND @anno IS NOT NULL)
		BEGIN

			/*Si los parámetros del grupo son válidos, recupera la información de las unidades académicas.*/
			IF (EXISTS (SELECT * FROM Grupo WHERE SiglaCurso = @siglaCurso AND NumGrupo = @numeroGrupo AND Semestre = @semestre AND Anno = @anno))
			BEGIN

				INSERT INTO @uasTemp
				SELECT	U.Codigo, U.Nombre
				FROM	UnidadAcademica AS U
				WHERE	EXISTS (
									SELECT *
									FROM Inscrita_en AS I
									JOIN Pertenece_a AS PE ON I.CodCarrera = PE.CodCarrera
									WHERE I.CodUnidadAc = U.Codigo AND PE.SiglaCurso = @siglaCurso /*Solo de unidades académicas a las que pertenece el curso del grupo dado como parámetro.*/
								)
				UNION
				SELECT CodigoUA, NombreUA
				FROM @uasGrupo;

				/*Limpia las variables y actualiza uasGrupo con el nuevo resultado.*/
				DELETE FROM @uasGrupo;

				INSERT INTO @uasGrupo
				SELECT CodigoUA, NombreUA
				FROM @uasTemp;

				DELETE FROM @uasTemp;

			END;

		END;

		FETCH NEXT FROM G INTO @siglaCurso, @numeroGrupo, @semestre, @anno;

	END;

	CLOSE G;
	DEALLOCATE G;

	RETURN;

END;