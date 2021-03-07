/*TAM 11.1: Procedimiento que devuelve true si la persona dada está relacionada con la carrera y énfasis
dados de alguna manera a través de uno o más perfiles, o false en caso contrario.*/

CREATE PROCEDURE [dbo].[EstaEnCarreraYEnfasis]
	@correoUsuario VARCHAR(50),
	@codCarrera VARCHAR(10),
	@codEnfasis VARCHAR(10),
	@resultado BIT OUTPUT
AS
	IF EXISTS(
		SELECT *
		FROM UsuarioPerfil
		WHERE Usuario = @correoUsuario AND CodCarrera = @codCarrera AND CodEnfasis = @codEnfasis
	)
SET @resultado = 1
ELSE
SET @resultado = 0

