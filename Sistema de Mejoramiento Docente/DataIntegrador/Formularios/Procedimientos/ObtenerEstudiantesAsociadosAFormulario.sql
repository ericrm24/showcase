/**
Función almacenada que recibe unas serie de parámetros,
para filtrar por unidad academica, carrera-enfasis, profesor,
grupo; y sus respectivas combinaciones. Devuelve una tabla
que contiene todos grupos
*/
CREATE PROCEDURE [dbo].[ObtenerEstudiantesAsociados]
(
	@CodigoUnidadAcademica varchar(10),
	@CodigoCarrera	varchar(10),
	@CodigoEnfasis	varchar(10),
	@SiglaCurso		varchar(10),
	@NumGrupo		tinyint,
	@Semestre		tinyint,
	@Anno			int,
	@CorreoProfesor	varchar(50)
)
AS
	--Unidad academica
	IF (@CodigoUnidadAcademica IS NOT NULL AND @CodigoCarrera IS NULL AND @CodigoEnfasis IS NULL AND @SiglaCurso IS NULL AND @NumGrupo IS NULL AND	@Semestre IS NULL AND @Anno IS NULL AND @CorreoProfesor IS NULL)
		SELECT * FROM Estudiante e
									JOIN UnidadAcademica ua on ua.Codigo = @CodigoCarrera
									JOIN Matriculado_en me ON me.CorreoEstudiante = e.Correo
									JOIN Inscrita_en ie ON ua.Codigo = ie.CodUnidadAc 
									JOIN Carrera ca ON ca.Codigo = ie.CodCarrera 
									JOIN Enfasis en ON en.CodCarrera = ca.Codigo
									JOIN Pertenece_a pa ON pa.CodCarrera = ca.Codigo AND pa.CodEnfasis = en.Codigo
									JOIN Curso c ON c.Sigla = pa.SiglaCurso AND me.SiglaCurso = c.Sigla
									JOIN Grupo g ON g.SiglaCurso = c.Sigla AND g.NumGrupo = me.NumGrupo
									JOIN Enfasis ON en.CodCarrera = ca.Codigo
									
		WHERE ua.Codigo = @CodigoUnidadAcademica


	-- Carrera-Enfasis
	IF (@CodigoUnidadAcademica IS NULL AND @CodigoCarrera IS NOT NULL AND @CodigoEnfasis IS NOT NULL AND @SiglaCurso IS NULL AND @NumGrupo IS NULL AND	@Semestre IS NULL AND @Anno IS NULL AND @CorreoProfesor IS NULL )
		SELECT e.Correo, e.Carne FROM UnidadAcademica ua 
									JOIN Inscrita_en ie ON ua.Codigo = ie.CodUnidadAc 
									JOIN Carrera ca ON ca.Codigo = ie.CodCarrera 
									JOIN Enfasis en ON en.CodCarrera = ca.Codigo
									JOIN Pertenece_a pa ON pa.CodCarrera = ca.Codigo AND pa.CodEnfasis = en.Codigo
									JOIN Curso c ON c.Sigla = pa.SiglaCurso
									JOIN Grupo g ON g.SiglaCurso = c.Sigla
									JOIN Matriculado_en me ON me.SiglaCurso = c.Sigla
									JOIN Estudiante e ON me.CorreoEstudiante = e.Correo

		WHERE ca.Codigo = @CodigoCarrera AND en.Codigo = @CodigoEnfasis

	--Profesor 
	IF (@CodigoUnidadAcademica IS NULL AND @CodigoCarrera IS  NULL AND @CodigoEnfasis IS  NULL AND @SiglaCurso IS NULL AND @NumGrupo IS NULL AND @Semestre IS NULL AND @Anno IS NULL AND @CorreoProfesor IS NOT NULL)
			SELECT * FROM Estudiante e JOIN Matriculado_en me ON me.CorreoEstudiante = e.Correo
										JOIN Imparte im on im.SiglaCurso = me.SiglaCurso
										JOIN Profesor p ON p.Correo = im.CorreoProfesor
										JOIN Grupo g ON g.SiglaCurso = im.SiglaCurso AND g.NumGrupo = im.NumGrupo AND g.Semestre = im.Semestre AND g.Anno = im.Anno
			WHERE p.Correo = @CorreoProfesor

	--UA-Enfasis
	IF (@CodigoUnidadAcademica IS NOT NULL AND @CodigoCarrera IS NOT NULL AND @CodigoEnfasis IS NOT NULL AND @SiglaCurso IS NULL AND @NumGrupo IS NULL AND	@Semestre IS NULL AND @Anno IS NULL AND @CorreoProfesor IS NULL )
			SELECT * FROM Estudiante e JOIN Matriculado_en me ON me.CorreoEstudiante = e.Correo
									JOIN UnidadAcademica ua on ua.Codigo = @CodigoCarrera
									JOIN Inscrita_en ie ON ua.Codigo = ie.CodUnidadAc 
									JOIN Carrera ca ON ca.Codigo = ie.CodCarrera 
									JOIN Enfasis en ON en.CodCarrera = ca.Codigo
									JOIN Pertenece_a pa ON pa.CodCarrera = ca.Codigo AND pa.CodEnfasis = en.Codigo
									JOIN Curso c ON c.Sigla = pa.SiglaCurso AND me.SiglaCurso = c.Sigla
									JOIN Grupo g ON g.SiglaCurso = c.Sigla AND g.NumGrupo = me.NumGrupo
			WHERE ua.Codigo = @CodigoUnidadAcademica AND ca.Codigo = @CodigoCarrera AND en.Codigo = @CodigoEnfasis

	-- UA-Profesor
	IF (@CodigoUnidadAcademica IS NOT NULL AND @CodigoCarrera IS  NULL AND @CodigoEnfasis IS  NULL AND @SiglaCurso IS NULL AND @NumGrupo IS NULL AND	@Semestre IS NULL AND @Anno IS NULL AND @CorreoProfesor IS NOT NULL)
			SELECT * FROM Estudiante e JOIN Matriculado_en me ON me.CorreoEstudiante = e.Correo
									JOIN UnidadAcademica ua on ua.Codigo = @CodigoCarrera
									JOIN Inscrita_en ie ON ua.Codigo = ie.CodUnidadAc 
									JOIN Carrera ca ON ca.Codigo = ie.CodCarrera 
									JOIN Enfasis en ON en.CodCarrera = ca.Codigo
									JOIN Pertenece_a pa ON pa.CodCarrera = ca.Codigo AND pa.CodEnfasis = en.Codigo
									JOIN Curso c ON c.Sigla = pa.SiglaCurso AND me.SiglaCurso = c.Sigla
									JOIN Grupo g ON g.SiglaCurso = c.Sigla AND g.NumGrupo = me.NumGrupo
									JOIN Imparte im on im.SiglaCurso = me.SiglaCurso
									JOIN Profesor p ON im.CorreoProfesor = p.Correo
			WHERE p.Correo = @CorreoProfesor AND ua.Codigo =  @CodigoUnidadAcademica

	-- Carrera-Enfasis-Profesor
	IF  (@CodigoUnidadAcademica IS NULL AND @CodigoCarrera IS NOT NULL AND @CodigoEnfasis IS NOT NULL AND @SiglaCurso IS NULL AND @NumGrupo IS NULL AND	@Semestre IS NULL AND @Anno IS NULL AND @CorreoProfesor IS NOT NULL)
			SELECT * FROM Estudiante e JOIN Matriculado_en me ON me.CorreoEstudiante = e.Correo
									JOIN UnidadAcademica ua on ua.Codigo = @CodigoCarrera
									JOIN Inscrita_en ie ON ua.Codigo = ie.CodUnidadAc 
									JOIN Carrera ca ON ca.Codigo = ie.CodCarrera 
									JOIN Enfasis en ON en.CodCarrera = ca.Codigo
									JOIN Pertenece_a pa ON pa.CodCarrera = ca.Codigo AND pa.CodEnfasis = en.Codigo
									JOIN Curso c ON c.Sigla = pa.SiglaCurso AND me.SiglaCurso = c.Sigla
									JOIN Grupo g ON g.SiglaCurso = c.Sigla AND g.NumGrupo = me.NumGrupo
									JOIN Imparte im on im.SiglaCurso = me.SiglaCurso
									JOIN Profesor p ON im.CorreoProfesor = p.Correo
			WHERE ca.Codigo = @CodigoCarrera AND en.Codigo = @CodigoEnfasis AND p.Correo = @CorreoProfesor

	-- Todos los que vengan con un grupo
	IF ( @SiglaCurso IS NOT NULL AND @NumGrupo IS NOT NULL AND	@Semestre IS NOT NULL AND @Anno IS NOT NULL )
		SELECT * FROM Estudiante e JOIN Matriculado_en me ON me.CorreoEstudiante = e.Correo
									JOIN UnidadAcademica ua ON ua.Codigo IS NOT NULL 
									JOIN Inscrita_en ie ON ua.Codigo = ie.CodUnidadAc 
									JOIN Carrera ca ON ca.Codigo = ie.CodCarrera 
									JOIN Enfasis en ON en.CodCarrera = ca.Codigo
									JOIN Pertenece_a pa ON pa.CodCarrera = ca.Codigo AND pa.CodEnfasis = en.Codigo
									JOIN Curso C ON c.Sigla = pa.SiglaCurso AND me.SiglaCurso = c.Sigla
									JOIN Grupo g ON g.SiglaCurso = c.Sigla AND g.NumGrupo = me.NumGrupo
		WHERE g.Anno = @Anno AND g.SiglaCurso = @SiglaCurso AND g.Semestre = @Semestre
RETURN 0