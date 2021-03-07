/*TAM-3.2 y 3.6: Listar usuarios con sus respectivos permisos según su énfasis y perfil elegido en la página de administración de 
permisos y perfiles. Este procedimiento devuelve 1 si el usuario con el correo ingresado tiene el permiso indicado, 0 
en caso contrario.*/

CREATE PROCEDURE [dbo].[TienePermisoSinEnfasisNiCarrera]
(
	@correoUsuario NVARCHAR(50),
	@perfil NVARCHAR(50),
	@permiso INT,
	@resultado BIT OUTPUT
)
AS
	IF (@permiso in 
	(select PP.PermisoId from UsuarioPerfil as UP JOIN PerfilPermiso as PP ON 
		UP.Perfil = PP.Perfil AND 
		UP.CodCarrera = PP.CodCarrera AND 
		UP.CodEnfasis = PP.CodEnfasis
	 where UP.Usuario = @correoUsuario AND PP.Perfil = @perfil
	)
)
	SET @resultado = 1
ELSE
	SET @resultado = 0

