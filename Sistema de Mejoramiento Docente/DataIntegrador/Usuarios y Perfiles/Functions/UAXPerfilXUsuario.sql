/* TAM 12.2 Funcion que retorna las Unidades Academicas que puede ver un usuario dado su perfil */
CREATE FUNCTION [dbo].[UAXPerfilXUsuario]
(
	@usuarioActual VARCHAR(50),
	@perfilActual VARCHAR(50)
)
RETURNS @returntable TABLE
(
	CodigoUA VARCHAR(10),
	NombreUA VARCHAR(50)
)
AS
BEGIN
	INSERT @returntable
	SELECT DISTINCT UA.Codigo, UA.Nombre
	-- Join de USUARIOPERFIL - CARRERA - INSCRITA_EN - UNIDADACADEMICA
	FROM UsuarioPerfil AS UP JOIN Carrera AS C ON UP.CodCarrera = C.Codigo
					  JOIN Inscrita_en AS I ON C.Codigo = I.CodCarrera
					  JOIN UnidadAcademica AS UA ON I.CodUnidadAc = UA.Codigo
	WHERE UP.Usuario = @usuarioActual AND UP.Perfil = @perfilActual

	RETURN
END