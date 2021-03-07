/*
Retorna una tabla con el correo, primer y segundo nombre, primer y segundo apellido de todos los profesores que imparten en los grupos dados como parámetro.
Esta información se necesita para filtrar profesores.
*/
CREATE FUNCTION ObtenerProfesoresGrupo (@Grupos FiltroGrupos READONLY)
RETURNS @profesoresGrupo TABLE
(
	Correo		VARCHAR(50) PRIMARY KEY,	/*Correo del profesor*/
	Nombre1		VARCHAR(15) NOT NULL,		/*Primer nonmbre del profesor*/
    Nombre2		VARCHAR(15) NULL,			/*Segundo nonmbre del profesor*/
    Apellido1	VARCHAR(15) NOT NULL,		/*Primer apellido del profesor*/
    Apellido2	VARCHAR(15) NULL			/*Segundo apellido del profesor*/
)
AS
BEGIN

	/*Almacena resultados de forma temporal para poder hacer las uniones.*/
	DECLARE @profesoresTemp TABLE
	(
		Correo		VARCHAR(50) PRIMARY KEY,	/*Correo del profesor*/
		Nombre1		VARCHAR(15) NOT NULL,		/*Primer nonmbre del profesor*/
		Nombre2		VARCHAR(15) NULL,			/*Segundo nonmbre del profesor*/
		Apellido1	VARCHAR(15) NOT NULL,		/*Primer apellido del profesor*/
		Apellido2	VARCHAR(15) NULL			/*Segundo apellido del profesor*/
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

		/*Si los parámetros del grupo no son nulos, verifica si son válidos y recupera la información de los profesores.*/
		IF (@siglaCurso IS NOT NULL AND @numeroGrupo IS NOT NULL AND @semestre IS NOT NULL AND @anno IS NOT NULL)
		BEGIN

			/*Si los parámetros del grupo son válidos, recupera la información de los profesores.*/
			IF (EXISTS (SELECT * FROM Grupo WHERE SiglaCurso = @siglaCurso AND NumGrupo = @numeroGrupo AND Semestre = @semestre AND Anno = @anno))
			BEGIN

				INSERT INTO @profesoresTemp
				SELECT	DISTINCT P.Correo, P.Nombre1, P.Nombre2, P.Apellido1, P.Apellido2
				FROM	Persona AS P JOIN Imparte AS Im ON P.Correo = Im.CorreoProfesor
				WHERE	Im.SiglaCurso = @siglaCurso AND Im.NumGrupo = @numeroGrupo AND Im.Semestre = @semestre AND Im.Anno = @anno /*Solo de profesores que imparten el grupo.*/
				UNION
				SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
				FROM @profesoresGrupo;

				/*Limpia las variables y actualiza profesoresGrupo con el nuevo resultado.*/
				DELETE FROM @profesoresGrupo;

				INSERT INTO @profesoresGrupo
				SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
				FROM @profesoresTemp;

				DELETE FROM @profesoresTemp;

			END;

		END;

		FETCH NEXT FROM G INTO @siglaCurso, @numeroGrupo, @semestre, @anno;

	END;

	CLOSE G;
	DEALLOCATE G;

	RETURN;

END;