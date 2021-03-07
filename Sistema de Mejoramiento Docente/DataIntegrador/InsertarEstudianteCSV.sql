CREATE PROCEDURE [dbo].[InsertarEstudianteCSV]
	@Correo varchar(50)
AS
BEGIN
	INSERT INTO Estudiante(Correo)
	VALUES (@Correo)
END
