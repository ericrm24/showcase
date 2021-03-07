CREATE PROCEDURE [dbo].[AsociarPreguntaConSeccion]
	@CodigoSeccion varchar(8),
	@CodigoPregunta varchar(8)
AS
	BEGIN TRY
		BEGIN TRANSACTION AsociarPreguntaConSeccion
			DECLARE @orden INT
			SELECT @orden = count(*)
			FROM Seccion_tiene_pregunta sp
			WHERE sp.SCodigo = @CodigoSeccion;

			MERGE INTO Seccion_tiene_pregunta AS Target
			USING (VALUES
				(@CodigoSeccion, @CodigoPregunta, @Orden)
			)
			AS Source(SCodigo, PCodigo, Orden)
			ON Target.SCodigo = Source.SCodigo AND Target.PCodigo = Source.PCodigo
			WHEN NOT MATCHED BY Target THEN
			INSERT (SCodigo, PCodigo, Orden)
			VALUES (SCodigo, PCodigo, Orden);
		COMMIT TRANSACTION AsociarPreguntaConSeccion
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION AsociarPreguntaConSeccion
	END CATCH
RETURN 0
