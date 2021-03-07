CREATE PROCEDURE [dbo].[InsertarCursoCSV]
	@Sigla varchar(10),
	@Nombre varchar(50)
AS
BEGIN
	INSERT INTO Curso(Sigla, Nombre)
	VALUES (@Sigla, @Nombre)
END
