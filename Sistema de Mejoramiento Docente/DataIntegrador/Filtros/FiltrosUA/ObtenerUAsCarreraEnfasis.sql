/*
Retorna una tabla con el código y nombre de las unidades académicas a las que pertenecen las carreras y énfasis dadas como parámetro.
Esta información se necesita para filtrar unidades académicas.
*/
CREATE FUNCTION ObtenerUAsCarreraEnfasis (@CarrerasEnfasis FiltroCarrerasEnfasis READONLY)
RETURNS @uasCarreraEnfasis TABLE
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

	DECLARE @codigoCarrera VARCHAR(10);
	DECLARE @codigoEnfasis VARCHAR(10);
	DECLARE CE CURSOR FOR SELECT CodigoCarrera, CodigoEnfasis FROM @CarrerasEnfasis;

	OPEN CE;
	FETCH NEXT FROM CE INTO @codigoCarrera, @codigoEnfasis;
	
	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si el código de la carrera y énfasis no son nulos, verifica si son válidos y recupera la información de las unidades académicas.*/
		IF (@codigoCarrera IS NOT NULL AND @codigoEnfasis IS NOT NULL)
		BEGIN

			/*Si el código de la carrera y énfasis son válidos, recupera la información de las unidades académicas.*/
			IF (EXISTS (SELECT * FROM Enfasis WHERE CodCarrera = @codigoCarrera AND Codigo = @codigoEnfasis))
			BEGIN

				INSERT INTO @uasTemp
				SELECT	U.Codigo, U.Nombre
				FROM	UnidadAcademica AS U JOIN Inscrita_en AS I ON U.Codigo = I.CodUnidadAc
				WHERE	I.CodCarrera = @codigoCarrera /*Solo de unidades académicas a las que pertenecen los énfasis dados como parámetro.*/
				UNION
				SELECT CodigoUA, NombreUA
				FROM @uasCarreraEnfasis;

				/*Limpia las variables y actualiza uasCarreraEnfasis con el nuevo resultado.*/
				DELETE FROM @uasCarreraEnfasis;

				INSERT INTO @uasCarreraEnfasis
				SELECT CodigoUA, NombreUA
				FROM @uasTemp;

				DELETE FROM @uasTemp;

			END;

		END;

		FETCH NEXT FROM CE INTO @codigoCarrera, @codigoEnfasis;

	END;

	CLOSE CE;
	DEALLOCATE CE;

	RETURN;

END;
