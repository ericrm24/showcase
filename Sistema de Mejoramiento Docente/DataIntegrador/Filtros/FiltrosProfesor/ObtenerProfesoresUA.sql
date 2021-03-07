/*
Retorna una tabla con el correo, primer y segundo nombre, primer y segundo apellido de todos los profesores que imparten en las unidades académicas dadas como parámetro.
Esta información se necesita para filtrar profesores.
*/
CREATE FUNCTION ObtenerProfesoresUA (@UnidadesAcademicas FiltroUnidadesAcademicas READONLY)
RETURNS @profesoresUA TABLE
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
	
	DECLARE @codigoUA VARCHAR(10);
	DECLARE UA CURSOR FOR SELECT CodigoUA FROM @UnidadesAcademicas;

	OPEN UA;
	FETCH NEXT FROM UA INTO @codigoUA;

	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si el código de la unidad académica no es nulo, verifica si es válido y recupera la información de los formularios.*/
		IF (@codigoUA IS NOT NULL)
		BEGIN

			/*Si el código de la unidad académica es válido, recupera la información de los formularios.*/
			IF (EXISTS (SELECT * FROM UnidadAcademica WHERE Codigo = @codigoUA))
			BEGIN

				INSERT INTO @profesoresTemp
				SELECT	DISTINCT P.Correo, P.Nombre1, P.Nombre2, P.Apellido1, P.Apellido2
				FROM	Persona AS P JOIN Imparte AS Im ON P.Correo = Im.CorreoProfesor
				WHERE	EXISTS	(
										SELECT *
										FROM UnidadAcademica AS U
										JOIN Inscrita_en AS I ON U.Codigo = I.CodUnidadAc
										JOIN Pertenece_a AS PE ON I.CodCarrera = PE.CodCarrera
										WHERE U.Codigo = @codigoUA AND PE.SiglaCurso = Im.SiglaCurso /*Solo de profesores para los cursos de la unidad académica.*/
									)
				UNION
				SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
				FROM @profesoresUA;

				/*Limpia las variables y actualiza profesoresUA con el nuevo resultado.*/
				DELETE FROM @profesoresUA;

				INSERT INTO @profesoresUA
				SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
				FROM @profesoresTemp;

				DELETE FROM @profesoresTemp;

			END;

		END;

		FETCH NEXT FROM UA INTO @codigoUA;

	END;

	CLOSE UA;
	DEALLOCATE UA;

	RETURN;

END;