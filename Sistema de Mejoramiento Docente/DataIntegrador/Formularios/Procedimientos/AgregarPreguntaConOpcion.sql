CREATE PROCEDURE [dbo].[AgregarPreguntaConOpcion]
	@cod varchar(8),
	@type char,
	@enunciado nvarchar(250),
	@justificacion varchar(50) = NULL,
	@NumeroError int = NULL OUTPUT
AS
BEGIN
	-- Se define el nivel de aislamiento de read committed, ya que no queremos que nadie más pueda leer los datos hasta que 
	-- esta transacción haya committeado. 
	SET implicit_transactions off;
	SET transaction isolation level READ COMMITTED;

	BEGIN TRY
		-- Se crea la transacción ya que tiene que agregarse la pregunta a varias tablas, o a ninguna
		BEGIN TRANSACTION t1; 

			-- Primeramente se tiene que crear una pregunta, con su código y enunciado
			MERGE INTO Pregunta AS Target
			USING (VALUES
					(@cod, @enunciado, @type) 
			)
			AS Source ([Codigo],[Enunciado], [Tipo])
			ON Target.Codigo = Source.Codigo
			WHEN NOT MATCHED BY TARGET THEN
			INSERT (Codigo, Enunciado, Tipo)
			VALUES (@cod,@enunciado,@type);

			-- Una vez creada la pregunta, se puede crear su subclase, Pregunta_con_opciones
			MERGE INTO Pregunta_con_opciones AS Target
			USING (VALUES 
				(@cod, @justificacion)
			)
			As Source ([Codigo],[TituloCampoObservacion])
			ON Target.Codigo = Source.Codigo
			WHEN NOT MATCHED BY TARGET THEN 
			INSERT (Codigo, TituloCampoObservacion)
			VALUES (@cod,@justificacion);


			-- Se fija si es una pregunta con opciones de selección, es decir, Selección Única o Selección Múltiple
			IF @type = 'U' OR @type = 'M' 
			BEGIN
				MERGE INTO Pregunta_con_opciones_de_seleccion AS Target
				USING (VALUES
						(@cod) 
				)
				AS Source ([Codigo])
				ON Target.Codigo = Source.Codigo
				WHEN NOT MATCHED BY TARGET THEN
				INSERT (Codigo)
				VALUES (@cod);
			END;

		COMMIT TRANSACTION t1;
	END TRY
	BEGIN CATCH
		PRINT 'No se logró insertar la pregunta';
			SET @NumeroError = ERROR_NUMBER();
			THROW;
			ROLLBACK TRANSACTION
	END CATCH
END

