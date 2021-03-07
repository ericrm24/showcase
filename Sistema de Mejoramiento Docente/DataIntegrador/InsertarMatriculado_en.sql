
CREATE PROCEDURE [dbo].[InsertarMatriculado_en]
	@Correo varchar(50),
	@Sigla varchar(10),
	@NumGrupo tinyint,
	@Semestre tinyint,
	@Anno int
AS

BEGIN
	--Esta condicion revisa que no se intente matricular estudiantes no existentes/grupos no existentes
	IF(@Correo IN (SELECT Correo FROM Estudiante) AND EXISTS (SELECT * FROM Grupo WHERE SiglaCurso = @Sigla and NumGrupo = @NumGrupo and Semestre = @Semestre and Anno = @Anno))
	BEGIN
		INSERT INTO Matriculado_en(CorreoEstudiante, SiglaCurso, NumGrupo, Semestre, Anno)
		VALUES (@Correo, @Sigla, @NumGrupo, @Semestre, @Anno)
	END
END