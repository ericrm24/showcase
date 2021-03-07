/*TAM 3.4 Task 1 Procedimiento para asignar y desasignar un permiso a un perfil*/
CREATE PROCEDURE [dbo].[AgregarPerfilPermiso]
	@perfil VARCHAR(50),
	@idPermiso INT,
	@codCarrera VARCHAR(10),
	@codEnfasis VARCHAR(10),
	@tienePermiso bit
AS
BEGIN
	-- Se verifica si se debe asignar o desasignar el permiso
	IF (@tienePermiso = 1)
	BEGIN
		/*Se configura nivel de aislamiento serializable para evitar el problema de phantom
		read en la tabla PerfilPermiso al verificar si existe el permiso asignado a un perfil.*/
		SET IMPLICIT_TRANSACTIONS ON
		SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
		BEGIN TRY
			BEGIN TRANSACTION
			-- Si no existe el permiso en el perfil se asigna
			IF NOT EXISTS (SELECT * 
						   FROM PerfilPermiso 
						   WHERE Perfil = @perfil AND PermisoId = @idPermiso 
								 AND CodCarrera = @codCarrera AND CodEnfasis = @codEnfasis
						   )
			BEGIN
				INSERT INTO PerfilPermiso (Perfil, PermisoId, CodCarrera, CodEnfasis)
				VALUES (@perfil, @idPermiso, @codCarrera, @codEnfasis)
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
		-- Se desasigna el permiso del perfil
		DELETE
		FROM PerfilPermiso
		WHERE Perfil = @perfil AND PermisoId = @idPermiso
		      AND CodCarrera = @codCarrera AND CodEnfasis = @codEnfasis
	END
END
RETURN 0
