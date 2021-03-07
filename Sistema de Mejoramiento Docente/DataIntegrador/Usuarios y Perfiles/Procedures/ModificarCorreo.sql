/*TAM-2.4 - Editar Usuarios*/
CREATE PROCEDURE [dbo].[ModificarCorreo]
	@anterior varchar(50),
	@nuevo varchar(50),
	@resultado bit output
AS
--Actividad Supervisada 26/11
SET implicit_transactions OFF
-- Aislamiento en repeatable read para asegurar el correcto cambio de primary key a las tuplas de interés.
SET TRANSACTION isolation level REPEATABLE READ
BEGIN TRY
BEGIN TRANSACTION
BEGIN
	-- Cambiar correo si no existe ya en la base
	IF NOT EXISTS (SELECT TOP 1 Correo FROM [dbo].[Persona] WHERE Correo=@nuevo)
	BEGIN
		UPDATE Persona
		SET Correo = @nuevo
		WHERE Correo = @anterior

		SET @resultado = 1;
	END

	ELSE
		SET @resultado = 0;
END
COMMIT TRANSACTION
SET implicit_transactions on
SET TRANSACTION isolation level READ COMMITTED
RETURN 0
END TRY
BEGIN CATCH
    PRINT ERROR_MESSAGE()
	ROLLBACK TRANSACTION
	SET implicit_transactions on
	SET TRANSACTION isolation level READ COMMITTED
	RETURN 1
END CATCH