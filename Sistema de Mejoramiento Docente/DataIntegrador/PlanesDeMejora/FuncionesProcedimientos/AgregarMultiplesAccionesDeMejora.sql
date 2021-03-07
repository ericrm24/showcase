CREATE TYPE AccionDeMejoraParametro 
AS TABLE(
	codPlan int not null,
	nombreObj varchar(50) not null,
	descripcion varchar(250) not null,
	fechaInicio date,
	fechaFin date,
	codPlantilla int,
	borrado bit
	)
GO

CREATE PROCEDURE [dbo].[AgregarMultiplesAccionesDeMejora]
	@Acciones AccionDeMejoraParametro READONLY
AS
	DECLARE @STATE INT = 0
	BEGIN TRY
			BEGIN TRANSACTION
				INSERT INTO AccionDeMejora
				SELECT * FROM @Acciones;
			COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		SELECT @STATE = ERROR_NUMBER()
		ROLLBACK TRANSACTION
	END CATCH
RETURN @STATE
