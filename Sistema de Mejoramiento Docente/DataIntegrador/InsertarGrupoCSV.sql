CREATE PROCEDURE [dbo].[InsertarGrupoCSV]
	@SiglaCurso varchar(10),
	@NumGrupo tinyint,
	@Semestre tinyint,
	@Anno int
AS
BEGIN
	--El curso debe existir antes de insertar un grupo
	IF(EXISTS (SELECT * FROM Curso WHERE Sigla = @SiglaCurso))
	BEGIN
		INSERT INTO Grupo(SiglaCurso, NumGrupo, Semestre, Anno)
		VALUES (@SiglaCurso, @NumGrupo, @Semestre, @Anno)
	END
END

