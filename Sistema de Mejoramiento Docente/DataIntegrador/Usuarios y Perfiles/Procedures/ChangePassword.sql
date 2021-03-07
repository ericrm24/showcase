--TAM-1.5 - Password Reset

CREATE PROCEDURE [dbo].[ChangePassword]
	@username VARCHAR(50),
	@newpassword VARCHAR(64)
AS
--Actividad Supervisada 26/11
SET implicit_transactions OFF
-- Aislamiento en repeatable read para asegurarse de que el correo no se cambie durante las consultas a la tabla, pero permitiendo lecturas concurrentes.
SET TRANSACTION isolation level repeatable READ
begin try
begin TRANSACTION
begin
	DECLARE @salt VARCHAR(64)

	SELECT @salt=Salt
	FROM Usuario
	WHERE Username=@username

	UPDATE Usuario
	SET Password = HASHBYTES('SHA2_256', @newpassword+CAST(@salt AS VARCHAR(36)))
	Where username=@username

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
