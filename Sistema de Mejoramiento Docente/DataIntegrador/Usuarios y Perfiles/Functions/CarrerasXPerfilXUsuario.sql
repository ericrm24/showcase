/*TAM 3.1: Función almacenada que devuelve una lista de códigos de carreras en las que el usuario dado con 
la carrera seleccionada tiene perfiles asignados.*/
CREATE FUNCTION [dbo].[CarrerasXPerfilXUsuario]
(
	@correoUsuario VARCHAR(50),
	@perfil VARCHAR(50)
)
RETURNS @returntable TABLE
(
	codCarrera VARCHAR(10),
	nombreCarrera VARCHAR(50)
)
AS
BEGIN
	INSERT @returntable
	SELECT DISTINCT C.Codigo, C.Nombre FROM UsuarioPerfil AS UP JOIN Carrera AS C ON UP.CodCarrera = C.Codigo
	WHERE UP.Usuario = @correoUsuario AND UP.Perfil = @perfil
	RETURN
END