/* TAM 12.2 Funcion que retorna los profesores que puede ver un usuario dado su perfil */
CREATE FUNCTION [dbo].[ProfesoresXUsuarioXPerfil]
(
	@usuarioActual VARCHAR(50),
	@perfilActual VARCHAR(50)
)
RETURNS @returntable TABLE
(
	Correo VARCHAR(50), 
	Nombre1 VARCHAR(15), 
	Nombre2 VARCHAR(15), 
	Apellido1 VARCHAR(15), 
	Apellido2 VARCHAR(15)
)
AS
BEGIN
	-- Se guarda la carrera y el enfasis del usuario actual
	DECLARE @carreraActual VARCHAR(10)
	DECLARE @enfasisActual VARCHAR(10)	
	SET @carreraActual = (SELECT UA.codCarrera FROM UsuarioActual AS UA WHERE UA.CorreoUsuario = @usuarioActual)
	SET @enfasisActual = (SELECT UA.CodEnfasis FROM UsuarioActual AS UA WHERE UA.CorreoUsuario = @usuarioActual)
	
	DECLARE @tienePermiso bit 
	SET @tienePermiso = 1	

	-- TO-DO Cambiar este codigo de permiso por los refinados
	DECLARE @permiso int = 401

	-- Se verifica si el usuario tiene permiso
	IF (@permiso in 
		(SELECT PP.PermisoId FROM UsuarioPerfil AS UP JOIN PerfilPermiso AS PP ON 
			UP.Perfil = PP.Perfil AND 
			UP.CodCarrera = PP.CodCarrera AND 
			UP.CodEnfasis = PP.CodEnfasis
		 WHERE UP.Usuario = @usuarioActual AND UP.CodCarrera = @carreraActual AND UP.CodEnfasis = @enfasisActual AND PP.Perfil = @perfilActual
		)
	)
		SET @tienePermiso = 1
	ELSE
		SET @tienePermiso = 0

	IF @tienePermiso = 1
	BEGIN
		-- Puede ver los profesores con los que ha matriculado
		IF @perfilActual = 'Estudiante'
		BEGIN
			INSERT @returntable
			SELECT DISTINCT P.Correo, P.Nombre1, P.Nombre2, P.Apellido1, P.Apellido2
			-- JOIN de ESTUDIANTE - MATRICULADO_EN - GRUPO - IMPARTE - PROFESOR - PERSONA
			FROM Estudiante AS E JOIN Matriculado_en AS ME ON E.Correo = ME.CorreoEstudiante
								 JOIN Grupo AS G ON ME.SiglaCurso = G.SiglaCurso AND
													ME.NumGrupo = G.NumGrupo AND
													ME.Semestre = G.Semestre AND
													ME.Anno = G.Anno
								 JOIN Imparte AS I ON G.SiglaCurso = I.SiglaCurso AND
													  G.NumGrupo = I.NumGrupo AND
													  G.Semestre = I.Semestre AND
													  G.Anno = I.Anno
								 JOIN Profesor AS Prof ON I.CorreoProfesor = Prof.Correo
								 JOIN Persona AS P ON Prof.Correo = P.Correo
			WHERE E.Correo = @usuarioActual
		END
		ELSE
		BEGIN
			-- Puede ver todos los profesores existentes
			IF @perfilActual = 'Superusuario'
			BEGIN
				INSERT @returntable
				SELECT DISTINCT P.Correo, P.Nombre1, P.Nombre2, P.Apellido1, P.Apellido2
				FROM Persona AS P JOIN Imparte AS I ON P.Correo = I.CorreoProfesor
			END
			ELSE
				-- Solo puede verse el mismo
				IF @perfilActual = 'Profesor'
				BEGIN
					INSERT @returntable
					SELECT DISTINCT P.Correo, P.Nombre1, P.Nombre2, P.Apellido1, P.Apellido2
					FROM Persona AS P JOIN Imparte AS I ON P.Correo = I.CorreoProfesor
					WHERE P.Correo = @usuarioActual
				END
				ELSE
				BEGIN
					-- Cualquier otro perfil puede ver los profesores de su enfasis
					INSERT @returntable
					SELECT DISTINCT P.Correo, P.Nombre1, P.Nombre2, P.Apellido1, P.Apellido2
					-- JOIN de ENFASIS - PERTENECE_A - CURSO - GRUPO - IMPARTE - PROFESOR - PERSONA
					FROM Enfasis AS E JOIN Pertenece_a AS PA ON E.CodCarrera = PA.CodCarrera AND
																E.Codigo = PA.CodEnfasis
									  JOIN Curso AS C ON PA.SiglaCurso = C.Sigla
									  JOIN Grupo AS G ON C.Sigla = G.SiglaCurso
									  JOIN Imparte AS I ON G.SiglaCurso = I.SiglaCurso AND
														   G.NumGrupo = I.NumGrupo AND
														   G.Semestre = I.Semestre AND
														   G.Anno = I.Anno
									  JOIN Profesor AS Prof ON I.CorreoProfesor = Prof.Correo
									  JOIN Persona AS P ON Prof.Correo = P.Correo
					WHERE E.CodCarrera = @carreraActual AND E.Codigo = @enfasisActual
				END
		END
	END

	RETURN
END
