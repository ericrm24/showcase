CREATE PROCEDURE [dbo].[InsertarFuncionarioCSV]
	@Correo varchar(50)
AS
BEGIN
	INSERT INTO Funcionario(Correo)
	VALUES (@Correo)
END
