/* TAM-3.5 Revisa si el usuario tiene el perfil en el énfasis indicado */

CREATE PROCEDURE [dbo].[TienePerfilEnElEnfasis]
	@username VARCHAR(50),
	@perfil VARCHAR(50),
	@codCarrera VARCHAR(10),
	@codEnfasis VARCHAR(10),
	@tienePerfil BIT OUTPUT
AS
BEGIN
	IF (@username in 
		(SELECT Usuario FROM UsuarioPerfil
		WHERE Usuario = @username AND
			Perfil = @perfil AND
			CodCarrera = @codCarrera AND
			CodEnfasis = @codEnfasis
		)
	)
		SET @tienePerfil = 1
	ELSE
		SET @tienePerfil = 0
END