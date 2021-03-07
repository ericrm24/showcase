CREATE PROCEDURE [dbo].[InsertarImparte]
	@CorreoProfesor VARCHAR(50),
	@SiglaCurso VARCHAR(10),
	@NumGrupo TINYINT,
	@Semestre TINYINT,
	@Anno INT
AS
BEGIN
	--Esta condicion revisa que no se intente insertar profesores no existentes/grupos no existentes
	IF(@CorreoProfesor IN (SELECT Correo FROM Profesor) AND EXISTS (SELECT * FROM Grupo WHERE SiglaCurso = @SiglaCurso and NumGrupo = @NumGrupo and Semestre = @Semestre and Anno = @Anno))
	BEGIN
		insert into Imparte (CorreoProfesor, SiglaCurso, NumGrupo, Semestre,Anno)
		VALUES ( @CorreoProfesor, @SiglaCurso, @NumGrupo, @Semestre, @Anno)
	END
END