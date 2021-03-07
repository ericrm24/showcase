/*TAM 3.1: Función almacenada que devuelve una lista de perfiles que tiene el usuario dado.*/
CREATE FUNCTION [dbo].[PerfilesXUsuario]
(
	@correoUsuario VARCHAR(50)
)
RETURNS @returntable TABLE
(
	NombrePefil VARCHAR(50)
)
AS
BEGIN
	INSERT @returntable
	SELECT DISTINCT UP.Perfil FROM UsuarioPerfil AS UP JOIN Usuario AS U ON UP.Usuario = U.Username
	WHERE U.Username = @correoUsuario
	RETURN
END
