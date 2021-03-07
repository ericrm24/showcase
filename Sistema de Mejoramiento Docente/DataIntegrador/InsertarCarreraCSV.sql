CREATE PROCEDURE [dbo].[InsertarCarreraCSV]
	@Cod varchar(10),
	@Nombre varchar(50)
AS
BEGIN
	INSERT INTO Carrera(Codigo, Nombre)
	VALUES (@Cod, @Nombre)
END
