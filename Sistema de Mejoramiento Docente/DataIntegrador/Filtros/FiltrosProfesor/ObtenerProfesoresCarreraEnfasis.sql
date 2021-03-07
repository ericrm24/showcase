/*
Retorna una tabla con el correo, primer y segundo nombre, primer y segundo apellido de todos los profesores que imparten en las carreras y énfasis dadas como parámetro.
Esta información se necesita para filtrar profesores.
*/
CREATE FUNCTION ObtenerProfesoresCarreraEnfasis (@CarrerasEnfasis FiltroCarrerasEnfasis READONLY)
RETURNS @profesoresCarreraEnfasis TABLE
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

	DECLARE @codigoCarrera VARCHAR(10);
	DECLARE @codigoEnfasis VARCHAR(10);
	DECLARE CE CURSOR FOR SELECT CodigoCarrera, CodigoEnfasis FROM @CarrerasEnfasis;

	OPEN CE;
	FETCH NEXT FROM CE INTO @codigoCarrera, @codigoEnfasis;
	
	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si el código de la carrera y énfasis no son nulos, verifica si son válidos y recupera la información de los profesores.*/
		IF (@codigoCarrera IS NOT NULL AND @codigoEnfasis IS NOT NULL)
		BEGIN

			/*Si el código de la carrera y énfasis son válidos, recupera la información de los profesores.*/
			IF (EXISTS (SELECT * FROM Enfasis WHERE CodCarrera = @codigoCarrera AND Codigo = @codigoEnfasis))
			BEGIN

				INSERT INTO @profesoresTemp
				SELECT	DISTINCT P.Correo, P.Nombre1, P.Nombre2, P.Apellido1, P.Apellido2
				FROM	Persona AS P JOIN Imparte AS Im ON P.Correo = Im.CorreoProfesor
				WHERE	EXISTS	(
										SELECT *
										FROM UnidadAcademica AS U
										JOIN Inscrita_en AS I ON U.Codigo = I.CodUnidadAc
										JOIN Pertenece_a AS PE ON I.CodCarrera = PE.CodCarrera
										WHERE PE.CodCarrera = @codigoCarrera AND PE.CodEnfasis = @codigoEnfasis AND PE.SiglaCurso = Im.SiglaCurso /*Solo de profesores para los cursos que contiene el énfasis.*/
									)
				UNION
				SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
				FROM @profesoresCarreraEnfasis;

				/*Limpia las variables y actualiza profesoresCarreraEnfasis con el nuevo resultado.*/
				DELETE FROM @profesoresCarreraEnfasis;

				INSERT INTO @profesoresCarreraEnfasis
				SELECT Correo, Nombre1, Nombre2, Apellido1, Apellido2
				FROM @profesoresTemp;

				DELETE FROM @profesoresTemp;

			END;

		END;

		FETCH NEXT FROM CE INTO @codigoCarrera, @codigoEnfasis;

	END;

	CLOSE CE;
	DEALLOCATE CE;

	RETURN;

END;
