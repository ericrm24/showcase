CREATE PROCEDURE [dbo].[GuardarRespuestaAPreguntaLibre]
	@codFormulario varchar(8),
	@correo varchar(50),
	@siglaCurso varchar(6),
	@numGrupo tinyint,
	@anno int,
	@semestre tinyint,
	@fecha date,
	@codPregunta varchar(8),
	@codseccion varchar(8),
	@texto nvarchar(250) = NULL
AS
BEGIN
	-- Creaciòn de la transacciòn de la historia RIP-EDF1
	set implicit_transactions off;	
	set transaction isolation level repeatable read;
	BEGIN TRY
		BEGIN TRANSACTION
		MERGE INTO Responde_respuesta_libre AS Target
		USING (VALUES
			(@codFormulario, @correo, @siglaCurso, @numGrupo, @anno, @semestre, @fecha, @codPregunta, @codseccion, @texto)
		)
		AS SOURCE ([FCodigo],[Correo],[CSigla],[GNumero],[GAnno],[GSemestre],[Fecha],[PCodigo], [SCodigo], [Observacion])
		ON Target.FCodigo = Source.FCodigo and Target.Correo = Source.Correo and 
		   Target.CSigla  = Source.CSigla  and Target.GNumero = Source.GNumero and 
		   Target.GAnno = Source.GAnno and Target.GSemestre = Source.GSemestre and 
		   Target.Fecha = Source.Fecha and Target.PCodigo = Source.PCodigo and
		   Target.SCodigo = Source.SCodigo
		WHEN NOT MATCHED BY TARGET THEN
		INSERT ([FCodigo],[Correo],[CSigla],[GNumero],[GAnno],[GSemestre],[Fecha],[PCodigo], [SCodigo], [Observacion])
		VALUES (@codFormulario, @correo, @siglaCurso, @numGrupo, @anno, @semestre, @fecha, @codPregunta, @codseccion, @texto);

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		SELECT ERROR_NUMBER(), ERROR_MESSAGE()	
		ROLLBACK TRANSACTION
	END CATCH
END;