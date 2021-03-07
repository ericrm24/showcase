/*
Retorna una tabla con el código de formulario, sigla del curso, número de grupo, semestre, año, fecha de inicio, fecha de finalización de todos los formularios que pertenecen a los grupos dados como parámetro.
Esta información se necesita para filtrar respuestas a formulario.
*/
CREATE FUNCTION ObtenerFormulariosGrupo (@Grupos FiltroGrupos READONLY)
RETURNS @formulariosGrupo TABLE
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

	DECLARE @siglaCurso VARCHAR(10);
	DECLARE @numeroGrupo TINYINT;
	DECLARE @semestre TINYINT;
	DECLARE @anno INT;
	DECLARE G CURSOR FOR SELECT SiglaCurso, NumeroGrupo, Semestre, Anno FROM @Grupos;

	OPEN G;
	FETCH NEXT FROM G INTO @siglaCurso, @numeroGrupo, @semestre, @anno;
	
	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si los parámetros del grupo no son nulos, verifica si son válidos y recupera la información de los formularios.*/
		IF (@siglaCurso IS NOT NULL AND @numeroGrupo IS NOT NULL AND @semestre IS NOT NULL AND @anno IS NOT NULL)
		BEGIN

			/*Si los parámetros del grupo son válidos, recupera la información de los formularios.*/
			IF (EXISTS (SELECT * FROM Grupo WHERE SiglaCurso = @siglaCurso AND NumGrupo = @numeroGrupo AND Semestre = @semestre AND Anno = @anno))
			BEGIN

				INSERT INTO @formulariosTemp
				SELECT	PAP.FCodigo, F.Nombre, PAP.CSigla, PAP.GNumero, PAP.GSemestre, PAP.GAnno, PAP.FechaInicio, PAP.FechaFin
				FROM	Periodo_activa_por AS PAP JOIN Formulario AS F ON PAP.FCodigo = F.Codigo
				WHERE	PAP.CSigla = @siglaCurso AND PAP.GNumero = @numeroGrupo AND PAP.GSemestre = @semestre AND PAP.GAnno = @anno /*Solo de formularios que pertenecen al grupo.*/
				UNION
				SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
				FROM @formulariosGrupo;

				/*Limpia las variables y actualiza formulariosGrupo con el nuevo resultado.*/
				DELETE FROM @formulariosGrupo;

				INSERT INTO @formulariosGrupo
				SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
				FROM @formulariosTemp;

				DELETE FROM @formulariosTemp;

			END;

		END;

		FETCH NEXT FROM G INTO @siglaCurso, @numeroGrupo, @semestre, @anno;

	END;

	CLOSE G;
	DEALLOCATE G;

	RETURN;

END;