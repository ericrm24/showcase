CREATE PROCEDURE [dbo].[AsociarSeccionConFormulario]
	@CodigoFormulario VARCHAR(8),
	@CodigoSeccion VARCHAR(8)
AS
	BEGIN TRY
		BEGIN TRANSACTION AsociarSeccionConFormulario
			DECLARE @orden INT
			SELECT @orden = count(*)
			FROM Formulario_tiene_seccion fs
			WHERE fs.FCodigo = @CodigoFormulario;

			MERGE INTO Formulario_tiene_seccion AS Target
			USING (VALUES
				(@CodigoFormulario, @CodigoSeccion, @Orden)
			)
			AS Source(FCodigo, SCodigo, Orden)
			ON Target.FCodigo = Source.FCodigo AND Target.SCodigo = Source.SCodigo
			WHEN NOT MATCHED BY Target THEN
			INSERT (FCodigo, SCodigo, Orden)
			VALUES (FCodigo, SCodigo, Orden);
		COMMIT TRANSACTION AsociarSeccionConFormulario
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION AsociarSeccionConFormulario
	END CATCH
RETURN 0
