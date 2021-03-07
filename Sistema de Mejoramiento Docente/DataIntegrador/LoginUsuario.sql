CREATE PROCEDURE LoginUsuario
	(@pLoginName VARCHAR(50),
	 @pPassword VARCHAR(50),
	  @result INT OUTPUT)
AS
BEGIN

	DECLARE @userID VARCHAR(50), @state bit

	-- Check if username exists in database
	IF EXISTS (SELECT TOP 1 Username FROM [dbo].[Usuario] WHERE Username=@pLoginName)
	BEGIN
		-- Check if user is activated
		SELECT @state = Activo
		FROM Usuario WHERE Username=@pLoginName

		IF(@state = 1)
		BEGIN
		
			-- Verify username and password (encrypted)
			SET @userID=(SELECT Username FROM [dbo].[Usuario] 
			WHERE Username=@pLoginName AND Password=HASHBYTES('SHA2_256',
			@pPassword+CAST(Salt AS VARCHAR(36))))

			-- Incorrect password
			IF(@userID IS NULL)
				SET @result=2

			-- User validated
			ELSE
				SET @result=0

		END

		-- User is not activated
		ELSE
			SET @result = 3;
	END

	-- User not found
	ELSE
		SET @result=1
END;
