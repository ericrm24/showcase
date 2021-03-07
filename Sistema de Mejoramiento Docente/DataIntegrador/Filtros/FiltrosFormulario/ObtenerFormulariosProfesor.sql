/*
Retorna una tabla con el código de formulario, sigla del curso, número de grupo, semestre, año, fecha de inicio, fecha de finalización de todos los formularios que pertenecen a los profesores dados como parámetro.
Esta información se necesita para filtrar respuestas a formulario.
*/
CREATE FUNCTION ObtenerFormulariosProfesor (@CorreosProfesores FiltroProfesores READONLY)
RETURNS @formulariosProfesor TABLE
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

	DECLARE @correoProfesor VARCHAR(50);
	DECLARE P CURSOR FOR SELECT CorreoProfesor FROM @CorreosProfesores;

	OPEN P;
	FETCH NEXT FROM P INTO @correoProfesor;
	
	WHILE (@@FETCH_STATUS = 0)
	BEGIN

		/*Si el correo del profesor no es nulo, verifica si es válido y recupera la información de los formularios.*/
		IF (@correoProfesor IS NOT NULL)
		BEGIN

			/*Si el correo del profesor es válido, recupera la información de los formularios.*/
			IF (EXISTS (SELECT * FROM Profesor WHERE Correo = @correoProfesor))
			BEGIN

				INSERT INTO @formulariosTemp
				SELECT	PAP.FCodigo, F.Nombre, PAP.CSigla, PAP.GNumero, PAP.GSemestre, PAP.GAnno, PAP.FechaInicio, PAP.FechaFin
				FROM	Periodo_activa_por AS PAP JOIN Formulario AS F ON PAP.FCodigo = F.Codigo
				WHERE	EXISTS		(
										SELECT *
										FROM Imparte AS I
										WHERE I.CorreoProfesor = @correoProfesor /*Solo de formularios activados para el profesor con el correo parámetro de cierto grupo.*/
										AND I.SiglaCurso = PAP.CSigla
										AND I.NumGrupo = PAP.GNumero
										AND I.Semestre = PAP.GSemestre
										AND I.Anno = PAP.GAnno
									)
				UNION
				SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
				FROM @formulariosProfesor;

				/*Limpia las variables y actualiza formulariosProfesor con el nuevo resultado.*/
				DELETE FROM @formulariosProfesor;

				INSERT INTO @formulariosProfesor
				SELECT FCodigo, FNombre, CSigla, GNumero, GSemestre, GAnno, FechaInicio, FechaFin
				FROM @formulariosTemp;

				DELETE FROM @formulariosTemp;

			END;

		END;

		FETCH NEXT FROM P INTO @correoProfesor;

	END;

	CLOSE P;
	DEALLOCATE P;

	RETURN;

END;