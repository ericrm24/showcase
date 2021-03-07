-- Procedimiento para sacar los formularios de un estudiante
-- en un semestre definido por parámetros

-- Ejemplo de uso: 
-- EXEC dbo.ObtenerFormulariosPorSemestre @correoEstudiante = 'paco@mail.com', @anno = 2019, @semestre = 2

CREATE PROCEDURE [dbo].[ObtenerFormulariosPorSemestre]
	@correoEstudiante VARCHAR(50),
	@anno INT,
	@semestre TINYINT
AS
BEGIN
	IF(@correoEstudiante IS NOT NULL AND @anno IS NOT NULL AND @semestre IS NOT NULL)
	BEGIN
		-- Conseguir los formularios basado los cursos matriculados, el grupo y los formularios activos
		-- Condicionado por el semestre y el correo del estudiante
		SELECT DISTINCT f.Codigo, m.SiglaCurso, m.NumGrupo, m.Semestre, m.Anno, p.FechaInicio, p.FechaFin
		FROM Matriculado_en m 
		JOIN Activa_por a ON m.NumGrupo = a.GNumero AND m.SiglaCurso = a.CSigla AND m.Semestre = a.GSemestre AND m.Anno = a.GAnno
		JOIN Periodo_activa_por p ON a.CSigla = p.CSigla AND a.FCodigo = p.FCodigo AND a.GAnno = p.GAnno AND a.GNumero = p.GNumero AND a.GSemestre = p.GSemestre
		JOIN Formulario f ON f.Codigo = a.FCodigo
		WHERE m.CorreoEstudiante = @correoEstudiante AND m.Anno = @anno AND m.Semestre = @semestre
	END
END
