CREATE PROCEDURE [dbo].[CheckID]
	@identificacion VARCHAR(30), 
    @result bit OUTPUT
AS
BEGIN
	SET @result = 1
    SET NOCOUNT ON
	IF NOT EXISTS (SELECT TOP 1 Identificacion FROM [dbo].[Persona] WHERE Identificacion=@identificacion)
	BEGIN
		SET @result = 0
	END
END
