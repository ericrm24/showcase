/*
Retorna una tabla con el código de formulario, sigla del curso, número de grupo, semestre, año, fecha de inicio, fecha de finalización de todos los formularios que pertenecen las unidades académicas dadas como parámetro.
Esta información se necesita para filtrar respuestas a formulario.
*/
CREATE FUNCTION ObtenerFormulariosUA (@UnidadesAcademicas FiltroUnidadesAcademicas READONLY)
RETURNS @formulariosUA TABLE
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

				INSERT INTO @formulariosTemp
				SELECT	PAP.FCodigo, F.Nombre, PAP.CSigla, PAP.GNumero, PAP.GSemestre, PAP.GAnno, PAP.FechaInicio, PAP.FechaFin
				FROM	Periodo_activa_por AS PAP JOIN Formulario AS F ON PAP.FCodigo = F.Codigo
				WHERE	EXISTS		(
										SELECT *
										FROM UnidadAcademica AS U
										JOIN Inscrita_en AS I ON U.Codigo = I.CodUnidadAc
										JOIN Pertenece_a AS PE ON I.CodCarrera = PE.CodCarrera
										WHERE U.Codigo = @codigoUA AND PE.SiglaCurso = PAP.CSigla /*Solo de formularios activados para los cursos que contiene la unidad académica.*/
									)
				UNION
				SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
				FROM @formulariosUA;

				/*Limpia las variables y actualiza formulariosUA con el nuevo resultado.*/
				DELETE FROM @formulariosUA;

				INSERT INTO @formulariosUA
				SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
				FROM @formulariosTemp;

				DELETE FROM @formulariosTemp;

			END;

		END;

		FETCH NEXT FROM UA INTO @codigoUA;

	END;

	CLOSE UA;
	DEALLOCATE UA;

	RETURN;

END;