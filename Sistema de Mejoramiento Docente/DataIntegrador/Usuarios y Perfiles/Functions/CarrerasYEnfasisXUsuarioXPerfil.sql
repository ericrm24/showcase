/*TAM-12.2: Funcón que recibe un usuario y un perfil, y retorna una tabla con la lista de carreras y 
énfasis por cada carrera que ese usuario puede ver.*/

CREATE FUNCTION [dbo].[CarrerasYEnfasisXUsuarioXPerfil]
(
	@usuario VARCHAR(50),
	@perfil VARCHAR(50)
)
RETURNS @returntable TABLE
(
	NombreCarrera VARCHAR(50),
	CodCarrera VARCHAR(10),
	NombreEnfasis VARCHAR(50),
	CodEnfasis VARCHAR(10)
)
AS
BEGIN
	INSERT @returntable
	SELECT DISTINCT CAR.Nombre, CAR.Codigo, ENF.Nombre, ENF.Codigo FROM UsuarioPerfil 
		JOIN Carrera AS CAR ON Codigo = CodCarrera
		JOIN Enfasis AS ENF ON ENF.CodCarrera = CAR.Codigo
	WHERE Usuario = @usuario AND Perfil = @perfil
	RETURN
END
