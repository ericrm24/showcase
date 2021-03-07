/* TAM 12.2 Funcion que retorna los grupos que puede ver un usuario dado su perfil */
CREATE FUNCTION [dbo].[GruposXPerfilXUsuario]
(
	@usuarioActual VARCHAR(50),
	@perfilActual VARCHAR(50)
)
RETURNS @returntable TABLE
(
	Sigla VARCHAR(10), 
	Nombre VARCHAR(50), 
	NumGrupo TINYINT, 
	Semestre TINYINT, 
	Anno INT
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

	-- Si el usuario tiene permiso con el perfil actual se despliegan los grupos
	IF @tienePermiso = 1
		-- Solo puede ver cursos en los que esta matriculado
		IF @perfilActual = 'Estudiante'
		BEGIN
			INSERT @returntable
			SELECT C.Sigla, C.Nombre, G.NumGrupo, G.Semestre, G.Anno
			-- JOIN de USUARIO - PERSONA - ESTUDIANTE - MATRICULADO_EN - GRUPO - CURSO
			FROM Usuario AS U JOIN Persona AS P ON U.Username = P.Correo 
							  JOIN Estudiante AS E ON P.Correo = E.Correo
							  JOIN Matriculado_en AS ME ON E.Correo = ME.CorreoEstudiante
							  JOIN Grupo AS G ON ME.SiglaCurso = G.SiglaCurso AND
												 ME.NumGrupo = G.NumGrupo AND 
												 ME.Semestre = G.Semestre AND 
												 ME.Anno = G.Anno
							  JOIN Curso AS C ON G.SiglaCurso = C.Sigla
			WHERE U.Username = @usuarioActual
		END
		ELSE
			-- Solo puede ver cursos que imparte
			IF @perfilActual = 'Profesor' 
			BEGIN
			-- JOIN de USUARIO - PERSONA - FUNCIONARIO - PROFESOR - IMPARTE - GRUPO - CURSO
				INSERT @returntable
				SELECT C.Sigla, C.Nombre, G.NumGrupo, G.Semestre, G.Anno
				FROM Usuario AS U JOIN Persona AS P ON U.Username = P.Correo 
								  JOIN Funcionario AS F ON P.Correo = F.Correo
								  JOIN Profesor AS Prof ON F.Correo = Prof.Correo
								  JOIN Imparte AS I ON Prof.Correo = I.CorreoProfesor
								  JOIN Grupo AS G ON I.SiglaCurso = G.SiglaCurso AND
													 I.NumGrupo = G.NumGrupo AND
													 I.Semestre = G.Semestre AND
													 I.Anno = G.Anno
								  JOIN Curso AS C ON G.SiglaCurso = C.Sigla
				WHERE U.Username = @usuarioActual
			END
			ELSE 
				-- Puede ver todos los cursos existentes
				IF @perfilActual = 'Superusuario'
				BEGIN
					INSERT @returntable
					SELECT C.Sigla, C.Nombre, G.NumGrupo, G.Semestre, G.Anno
					FROM Curso AS C JOIN Grupo AS G ON C.Sigla = G.SiglaCurso
				END
				ELSE
				BEGIN
					-- Cualquier otro perfil puede ver los cursos de su enfasis
					INSERT @returntable
					SELECT C.Sigla, C.Nombre, G.NumGrupo, G.Semestre, G.Anno
					-- JOIN de ENFASIS - PERTENECE_A - CURSO - GRUPO
					FROM Enfasis AS E JOIN Pertenece_a AS PA ON E.CodCarrera = PA.CodCarrera AND E.Codigo = PA.CodEnfasis
									  JOIN Curso AS C ON PA.SiglaCurso = C.Sigla
									  JOIN Grupo AS G ON C.Sigla = G.SiglaCurso
					WHERE E.Codigo = @enfasisActual AND E.CodCarrera = @carreraActual
				END
	RETURN
END