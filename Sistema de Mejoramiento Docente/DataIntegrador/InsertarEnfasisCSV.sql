CREATE PROCEDURE [dbo].[InsertarEnfasisCSV]
	@CodCarrera varchar(10),
	@Codigo varchar(10),
	@Nombre varchar(50)
AS
BEGIN
	INSERT INTO Enfasis(CodCarrera, Codigo, Nombre)
	VALUES (@CodCarrera, @Codigo, @Nombre)
END
