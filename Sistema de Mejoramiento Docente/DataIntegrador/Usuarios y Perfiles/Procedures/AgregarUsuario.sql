CREATE PROCEDURE [dbo].[AgregarUsuario]
	@pLogin VARCHAR(50), 
    @pPassword VARCHAR(50),
	@activo bit,
    @estado bit OUTPUT
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @salt UNIQUEIDENTIFIER=NEWID()
    BEGIN TRY

        -- Check if not already in database
		IF NOT EXISTS (SELECT TOP 1 Username FROM [dbo].[Usuario] WHERE Username=@pLogin)
		BEGIN
			INSERT INTO dbo.[Usuario] (Username, Password, Salt, Activo)
			VALUES(@pLogin, HASHBYTES('SHA2_256', @pPassword+CAST(@salt AS VARCHAR(36))), @salt, @activo)

			SET @estado = 1
		END

		ELSE
			SET @estado = 0

    END TRY
    BEGIN CATCH
        SET @estado=ERROR_MESSAGE() 
    END CATCH

END



	
