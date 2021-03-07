/*
Retorna una tabla con el código de formulario, sigla del curso, número de grupo, semestre, año, fecha de inicio, fecha de finalización de todos los formularios que pertenecen a las carreras y énfasis dadas como parámetro.
Esta información se necesita para filtrar respuestas a formulario.
*/
CREATE FUNCTION ObtenerFormulariosCarreraEnfasis (@CarrerasEnfasis FiltroCarrerasEnfasis READONLY)
RETURNS @formulariosCarreraEnfasis TABLE
(
	FCodigo VARCHAR(8),		/*Código del formulario.*/
	FNombre NVARCHAR(250),	/*Nombre del formulario*/
	CSigla VARCHAR(10),		/*Sigla del curso.*/
	GNumero TINYINT,		/*Número de grupo.*/
	GSemestre TINYINT,		/*Número de semestre.*/
	GAnno INT,				/*Año.*/
	FechaInicio DATE,		/*Fecha de inicio del periodo de llenado el formulario.*/
	FechaFin DATE,			/*Fecha de finalización del periodo de llenado del formulario.*/
	PRIMARY KEY (FCodigo, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin)
)
AS
BEGIN

	/*Almacena resultados de forma temporal para poder hacer las uniones.*/
	DECLARE @formulariosTemp TABLE
	(
		FCodigo VARCHAR(8),		/*Código del formulario.*/
		FNombre NVARCHAR(250),	/*Nombre del formulario*/
		CSigla VARCHAR(10),		/*Sigla del curso.*/
		GNumero TINYINT,		/*Número de grupo.*/
		GSemestre TINYINT,		/*Número de semestre.*/
		GAnno INT,				/*Año.*/
		FechaInicio DATE,		/*Fecha de inicio del periodo de llenado el formulario.*/
		FechaFin DATE,			/*Fecha de finalización del periodo de llenado del formulario.*/
		PRIMARY KEY (FCodigo, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin)
	);

	DECLARE @codigoCarrera VARCHAR(10);
	DECLARE @codigoEnfasis VARCHAR(10);
	DECLARE CE CURSOR FOR SELECT CodigoCarrera, CodigoEnfasis FROM @CarrerasEnfasis;

	OPEN CE;
	FETCH NEXT FROM CE INTO @codigoCarrera, @codigoEnfasis;
	
	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si el código de la carrera y énfasis no son nulos, verifica si son válidos y recupera la información de los formularios.*/
		IF (@codigoCarrera IS NOT NULL AND @codigoEnfasis IS NOT NULL)
		BEGIN

			/*Si el código de la carrera y énfasis son válidos, recupera la información de los formularios.*/
			IF (EXISTS (SELECT * FROM Enfasis WHERE CodCarrera = @codigoCarrera AND Codigo = @codigoEnfasis))
			BEGIN

				INSERT INTO @formulariosTemp
				SELECT	PAP.FCodigo, F.Nombre, PAP.CSigla, PAP.GNumero, PAP.GSemestre, PAP.GAnno, PAP.FechaInicio, PAP.FechaFin
				FROM	Periodo_activa_por AS PAP JOIN Formulario AS F ON PAP.FCodigo = F.Codigo
				WHERE	EXISTS		(
										SELECT *
										FROM UnidadAcademica AS U
										JOIN Inscrita_en AS I ON U.Codigo = I.CodUnidadAc
										JOIN Pertenece_a AS PE ON I.CodCarrera = PE.CodCarrera
										WHERE PE.CodCarrera = @codigoCarrera AND PE.CodEnfasis = @codigoEnfasis AND PE.SiglaCurso = PAP.CSigla /*Solo de formularios activados para los cursos que contiene el énfasis.*/
									)
				UNION
				SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
				FROM @formulariosCarreraEnfasis;

				/*Limpia las variables y actualiza formulariosCarreraEnfasis con el nuevo resultado.*/
				DELETE FROM @formulariosCarreraEnfasis;

				INSERT INTO @formulariosCarreraEnfasis
				SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
				FROM @formulariosTemp;

				DELETE FROM @formulariosTemp;

			END;

		END;

		FETCH NEXT FROM CE INTO @codigoCarrera, @codigoEnfasis;

	END;

	CLOSE CE;
	DEALLOCATE CE;

	RETURN;

END;
