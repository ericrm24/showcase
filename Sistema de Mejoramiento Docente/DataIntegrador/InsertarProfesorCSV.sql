CREATE PROCEDURE [dbo].[InsertarProfesorCSV]
	@Correo varchar(50)
AS
BEGIN
	INSERT INTO Profesor(Correo)
	VALUES (@Correo)
END
