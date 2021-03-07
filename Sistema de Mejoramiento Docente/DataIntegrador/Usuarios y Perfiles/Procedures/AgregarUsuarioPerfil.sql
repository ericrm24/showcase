/*TAM 3.4 Task 1 Procedimiento para asignar y desasignar un perfil a un usuario*/
CREATE PROCEDURE [dbo].[AgregarUsuarioPerfil]
	@usuario VARCHAR(50),
	@perfil VARCHAR(50),
	@codCarrera	VARCHAR(10),
	@codEnfasis VARCHAR(10),
	@tienePerfil bit
AS
BEGIN
	SET IMPLICIT_TRANSACTIONS ON

	-- Se configura el nivel de aislamiento para esta transaccion
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE

	-- Se verifica si se debe asignar o desasignar el perfil
	IF (@tienePerfil = 1)
	BEGIN
		-- Uso de transacciones y manejo de errores para evitar el problema de phantom read
		BEGIN TRY
			BEGIN TRANSACTION
				-- Si no existe la asignacion del perfil al usuario entonces se asigna
				IF NOT EXISTS (SELECT * 
							   FROM UsuarioPerfil 
							   WHERE Usuario = @usuario AND Perfil = @perfil 
									 AND CodCarrera = @codCarrera AND CodEnfasis = @codEnfasis
							   )
					BEGIN
						INSERT INTO UsuarioPerfil (Usuario, Perfil, CodCarrera, CodEnfasis)
						VALUES (@usuario, @perfil, @codCarrera, @codEnfasis)
					END			
			COMMIT TRANSACTION
			SET TRANSACTION ISOLATION LEVEL READ COMMITTED
			SET IMPLICIT_TRANSACTIONS OFF
		END TRY
		BEGIN CATCH
			SET TRANSACTION ISOLATION LEVEL READ COMMITTED
			SET IMPLICIT_TRANSACTIONS OFF
			ROLLBACK TRANSACTION
		END CATCH
	END
	ELSE
	BEGIN
		-- Se desasigna el perfil al usuario
		DELETE
		FROM UsuarioPerfil
		WHERE Usuario = @usuario AND Perfil = @perfil 
		      AND CodCarrera = @codCarrera AND CodEnfasis = @codEnfasis
	END
END
