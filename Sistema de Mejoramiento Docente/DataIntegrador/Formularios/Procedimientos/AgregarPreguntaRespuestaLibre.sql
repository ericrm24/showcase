CREATE PROCEDURE [dbo].[AgregarPreguntaRespuestaLibre]
	@cod varchar(8),
	@type char,
	@enunciado nvarchar(250)
AS
BEGIN
	-- Se tiene que crear una pregunta, con su código, tipo y enunciado
	MERGE INTO Pregunta AS Target
	USING (VALUES
			(@cod, @enunciado, @type) 
	)
	AS Source ([Codigo],[Enunciado], [Tipo])
	ON Target.Codigo = Source.Codigo
	WHEN NOT MATCHED BY TARGET THEN
	INSERT (Codigo, Enunciado, Tipo)
	VALUES (@cod,@enunciado,@type);
END

