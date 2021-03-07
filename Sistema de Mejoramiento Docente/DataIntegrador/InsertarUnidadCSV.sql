CREATE PROCEDURE [dbo].[InsertarUnidadCSV]
	@CodUnidad varchar(10),
	@Nfacultad varchar(50)
AS
BEGIN
	INSERT INTO UnidadAcademica(Codigo, Nombre)
	VALUES (@CodUnidad, @Nfacultad)
END