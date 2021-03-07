/* TAM-3.2 Revisa los permisos asignados al perfil en el énfasis seleccionado */

CREATE PROCEDURE [dbo].[TienePermisoActivoEnEnfasis]
	@permisoId INT,
	@perfil VARCHAR(50),
	@codCarrera VARCHAR(10),
	@codEnfasis VARCHAR(10),
	@tieneActivo BIT OUTPUT
AS
BEGIN
	IF (@permisoId in
		(SELECT TOP 1 PermisoId FROM PerfilPermiso
		WHERE PermisoId = @permisoId AND
			Perfil = @perfil AND
			CodCarrera = @codCarrera AND
			CodEnfasis = @codEnfasis
		)
	)
		SET @tieneActivo = 1
	ELSE
		SET @tieneActivo = 0
END