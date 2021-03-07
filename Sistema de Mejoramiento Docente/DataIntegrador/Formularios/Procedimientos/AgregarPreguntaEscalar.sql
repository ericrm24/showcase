CREATE PROCEDURE [dbo].[AgregarPreguntaEscalar]
	@cod varchar(8),
	@type char,
	@enunciado nvarchar(250),
	@justificacion varchar(50) = NULL,
	@incremento int,
	@minimo int,
	@maximo int,
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


			-- Se crea la pregunta de escalar
			BEGIN
				MERGE INTO Escalar AS Target
				USING (VALUES
						(@cod, 	@incremento, @minimo, @maximo) 
				)
				AS Source ([Codigo], [Incremento], [Minimo], [Maximo])
				ON Target.Codigo = Source.Codigo
				WHEN NOT MATCHED BY TARGET THEN
				INSERT (Codigo, Incremento, Minimo, Maximo)
				VALUES (@cod, @incremento, @minimo, @maximo);
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

