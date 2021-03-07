/* Configura los perfiles con permisos por defecto en el énfasis dado */
CREATE PROCEDURE [dbo].[ConfigurarPerfilesDefault]
	@codCarrera varchar(10),
	@codEnfasis varchar(10)
AS
	MERGE INTO PerfilPermiso AS Target
	USING (VALUES
		-- Estudiante - Llenar formulario
		('Estudiante', 215, @codCarrera, @codEnfasis),
		-- Estudiante - Ver resultados de formularios de mis cursos
		('Estudiante', 401, @codCarrera, @codEnfasis),
		-- Administrador - Cargar datos desde CSV
		('Administrador', 501, @codCarrera, @codEnfasis),
		-- Coordinador de comisión de docencia - Ver resultados de formularios del énfasis
		('Coordinador de comisión de docencia', 402, @codCarrera, @codEnfasis),
		-- Coordinador de énfasis - Ver resultados de formularios del énfasis
		('Coordinador de énfasis', 402, @codCarrera, @codEnfasis),
		-- Director - Ver resultados de formularios del énfasis
		('Director', 402, @codCarrera, @codEnfasis)
	)
	AS SOURCE ([Perfil], [PermisoId], [CodCarrera], [CodEnfasis])
	ON Target.Perfil = SOURCE.Perfil AND Target.PermisoId = SOURCE.PermisoID AND Target.CodCarrera = SOURCE.CodCarrera AND Target.CodEnfasis = SOURCE.CodEnfasis
	WHEN NOT MATCHED BY TARGET THEN
	INSERT (Perfil, PermisoId, CodCarrera, CodEnfasis)
	VALUES (Perfil, PermisoId, CodCarrera, CodEnfasis);