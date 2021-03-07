CREATE PROCEDURE [dbo].[AgregarEnlaceSeguro]
	@usuarioAsociado  VARCHAR(50), 
	@urlReal varchar(1000),
	@expira datetime,
	@usos int,
	@reestablecerContrasenna bit,
    @resultadohash varchar(64) OUTPUT,
	@estado nvarchar(255) OUTPUT
AS
BEGIN TRY
BEGIN TRANSACTION	
BEGIN
	--Se crea un hash basándonos en la dirección provista y un Salt que creamos.
	--diferentes IF que siguen a continuación manejan diferentes permutaciones de parámetros
    SET NOCOUNT ON	
	DECLARE @salt UNIQUEIDENTIFIER=NEWID()
	SET @resultadohash = CONVERT (VARCHAR(64), HASHBYTES('SHA2_256', @urlReal+CAST(@salt AS VARCHAR(36))), 2)
	--No se provee usos ni fecha de expiración (se usa el default).
	IF @usos = 0 AND @expira IS NULL
	
	BEGIN
		INSERT INTO EnlaceSeguro ([Hash], UrlReal, UsuarioAsociado, ReestablecerContrasenna)
		values (@resultadohash, @urlReal, @usuarioAsociado, @reestablecerContrasenna)
		SET @estado = 1
	END
	ELSE
		BEGIN
		--Solo se provee usos.
		IF  @usos != 0 AND @expira IS NULL
			BEGIN
				INSERT INTO EnlaceSeguro ([Hash],Usos, UrlReal, UsuarioAsociado, ReestablecerContrasenna)
				values (@resultadohash, @usos, @urlReal, @usuarioAsociado, @reestablecerContrasenna)
				SET @estado = 1
			End
		ELSE
			BEGIN
			--Solo se provee fecha de expiración.
			IF @usos = 0 AND @expira IS NOT NULL
				BEGIN
					INSERT INTO EnlaceSeguro ([Hash],UrlReal,Expira, UsuarioAsociado, ReestablecerContrasenna)
					values (@resultadohash, @urlReal, @expira, @usuarioAsociado, @reestablecerContrasenna)
					SET @estado = 1
				End
			ELSE
				BEGIN
				--Se proveen todos los parámetros opcionales.
				IF @usos != 0 AND @expira IS NOT NULL
					BEGIN
						INSERT INTO EnlaceSeguro ([Hash],Usos,UrlReal,Expira, UsuarioAsociado, ReestablecerContrasenna)
						values (@resultadohash,@usos, @urlReal, @expira, @usuarioAsociado, @reestablecerContrasenna)
						SET @estado = 1
					END
				END
			END
		END
END
COMMIT TRANSACTION
END TRY
BEGIN CATCH
    SET @estado=ERROR_MESSAGE()
	ROLLBACK TRANSACTION
END CATCH
