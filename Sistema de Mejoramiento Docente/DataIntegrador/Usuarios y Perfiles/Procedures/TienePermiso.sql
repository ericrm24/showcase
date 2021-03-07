/*TAM-3.2 y 3.6: Interfaz de permisos para el resto de funcionalidades del sistema. Procedimiento Almacenado para saber si 
una persona tiene un determinado permiso, dado un código de carrera, un código de énfasis y un perfil seleccionado.
Devuelve 1 si la persona tiene el perfil con el permiso consultado, 0 en caso contrario.*/
CREATE PROCEDURE [dbo].[TienePermiso]
	@correoUsuario VARCHAR(50),
	@perfil VARCHAR(50),
	@codCarrera VARCHAR(10),
	@codEnfasis VARCHAR(10),
	@permiso INT,
	@resultado BIT OUTPUT
AS
	IF (@permiso in 
	(SELECT PP.PermisoId FROM UsuarioPerfil AS UP JOIN PerfilPermiso AS PP ON 
		UP.Perfil = PP.Perfil AND 
		UP.CodCarrera = PP.CodCarrera AND 
		UP.CodEnfasis = PP.CodEnfasis
	 WHERE UP.Usuario = @correoUsuario AND UP.CodCarrera = @codCarrera AND UP.CodEnfasis = @codEnfasis AND PP.Perfil = @perfil
	)
)
	SET @resultado = 1
ELSE
	SET @resultado = 0
