CREATE PROCEDURE [dbo].[PopularSeccionesConPreguntas]
@NumeroError int = NULL OUTPUT
AS
BEGIN
	
	-- No se quiere que se haga lectura de los datos hasta que no se hay hecho commit
	SET implicit_transactions off;
	SET transaction isolation level REPEATABLE READ;

	BEGIN TRY
		-- Transaccion que agrega dos secciones
		BEGIN TRANSACTION popularSecciones
			EXEC dbo.AgregarSeccion 
				@codigo = 'INFOPROF',
				@nombre = 'Sobre el profesor';

			EXEC dbo.AgregarSeccion 
				@codigo = 'SUPRUEBA',
				@nombre = 'Seccion con preguntas de Seleccion Unica';
			
			EXEC dbo.AgregarSeccion 
				@codigo = 'PERSONAL',
				@nombre = 'Sobre usted como estudiante del curso';

			MERGE INTO Seccion_tiene_pregunta AS Target
				USING (VALUES
						('INFOPROF','PROFES01',0),
						('INFOPROF','PROFES02',1),
						('INFOPROF','PROFES03',2),
						('PERSONAL','PERSNAL1',0),
						('PERSONAL','PERSNAL2',1),
						('PERSONAL','PERSNAL3',2)
				)
				AS Source ([SCodigo],[PCodigo],Orden)
				ON Target.SCodigo = Source.SCodigo AND Target.PCodigo = Source.PCodigo
				WHEN NOT MATCHED BY TARGET THEN
				INSERT (SCodigo, PCodigo, Orden)
				VALUES (SCodigo, PCodigo, Orden);
				
				COMMIT TRANSACTION popularSecciones;
			END TRY
	BEGIN CATCH
			PRINT 'No se logró agregar las secciones INFOPROF y Personal';
			SET @NumeroError = ERROR_NUMBER();
			THROW;
			ROLLBACK TRANSACTION
	END CATCH
	-- 

	-- Se crean las secciones del formulario de prueba CI0128G1
	-- Sección 1
	BEGIN TRY 
		BEGIN TRANSACTION PopularCI0128
			EXEC dbo.AgregarSeccion
				@codigo = 'CI0128S1',
				@nombre = 'Sobre el proyecto integrador de ingeniería de software y bases de datos';

			MERGE INTO Seccion_tiene_pregunta AS Target
				USING (VALUES
						('CI0128S1','CI0128P1',0),
						('CI0128S1','CI0128P2',1),
						('CI0128S1','CI0128P3',2)
				)
				AS Source ([SCodigo],[PCodigo],Orden)
				ON Target.SCodigo = Source.SCodigo AND Target.PCodigo = Source.PCodigo
				WHEN NOT MATCHED BY TARGET THEN
				INSERT (SCodigo, PCodigo, Orden)
				VALUES (SCodigo, PCodigo, Orden);

		-- Sección 2
		EXEC dbo.AgregarSeccion 
			@codigo = 'CI0128S2',
			@nombre = 'Sobre el profesor Cristian Quesada López';

		MERGE INTO Seccion_tiene_pregunta AS Target
			USING (VALUES
					('CI0128S2','CI0128P4',0),
					('CI0128S2','CI0128P5',1),
					('CI0128S2','CI0128P6',2),
					('CI0128S2','CI0128P7',3)
			)
			AS Source ([SCodigo],[PCodigo],Orden)
			ON Target.SCodigo = Source.SCodigo AND Target.PCodigo = Source.PCodigo
			WHEN NOT MATCHED BY TARGET THEN
			INSERT (SCodigo, PCodigo, Orden)
			VALUES (SCodigo, PCodigo, Orden);

		-- Sección 3 
		EXEC dbo.AgregarSeccion 
			@codigo = 'CI0128S3',
			@nombre = 'Sobre la profesora Alexandra Martínez';

		MERGE INTO Seccion_tiene_pregunta AS Target
			USING (VALUES
					('CI0128S3','CI0128P4',0),
					('CI0128S3','CI0128P5',1),
					('CI0128S3','CI0128P6',2),
					('CI0128S3','CI0128P7',3)
			)
			AS Source ([SCodigo],[PCodigo],Orden)
			ON Target.SCodigo = Source.SCodigo AND Target.PCodigo = Source.PCodigo
			WHEN NOT MATCHED BY TARGET THEN
			INSERT (SCodigo, PCodigo, Orden)
			VALUES (SCodigo, PCodigo, Orden);

			-- Sección 4 
			EXEC dbo.AgregarSeccion 
				@codigo = 'CI0128S4',
				@nombre = 'Sobre el profesor Marcelo Jenkins';

			MERGE INTO Seccion_tiene_pregunta AS Target
				USING (VALUES
						('CI0128S4','CI0128P4',0),
						('CI0128S4','CI0128P5',1),
						('CI0128S4','CI0128P6',2),
						('CI0128S4','CI0128P7',3),
						('CI0128S4','CI0128P8',4)
				)
				AS Source ([SCodigo],[PCodigo],Orden)
				ON Target.SCodigo = Source.SCodigo AND Target.PCodigo = Source.PCodigo
				WHEN NOT MATCHED BY TARGET THEN
				INSERT (SCodigo, PCodigo, Orden)
				VALUES (SCodigo, PCodigo, Orden);
			COMMIT TRANSACTION PopularCI0128
		END TRY
			BEGIN CATCH
			PRINT 'No se logró agregar el formulario CI0128';
			SET @NumeroError = ERROR_NUMBER();
			THROW;
			ROLLBACK TRANSACTION
		END CATCH

END