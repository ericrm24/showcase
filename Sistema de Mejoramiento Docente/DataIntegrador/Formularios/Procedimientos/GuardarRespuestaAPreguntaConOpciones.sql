CREATE PROCEDURE [dbo].[GuardarRespuestaAPreguntaConOpciones]
	@codFormulario varchar(8),
	@correo varchar(50),
	@siglaCurso varchar(6),
	@numGrupo tinyint,
	@anno int,
	@semestre tinyint,
	@fecha date,
	@codPregunta varchar(8),
	@codseccion varchar(8),
	@justificacion nvarchar(250) = NULL
AS
BEGIN
	MERGE INTO  Responde_respuesta_con_opciones  AS Target
	USING (VALUES
		(@codFormulario, @correo, @siglaCurso, @numGrupo, @anno, @semestre, @fecha, @codPregunta, @codseccion, @justificacion)
	)
	AS SOURCE ([FCodigo],[Correo],[CSigla],[GNumero],[GAnno],[GSemestre],[Fecha],[PCodigo], [SCodigo], [Observacion])
	ON Target.FCodigo = Source.FCodigo and Target.Correo = Source.Correo and 
	   Target.CSigla  = Source.CSigla  and Target.GNumero = Source.GNumero and 
	   Target.GAnno = Source.GAnno and Target.GSemestre = Source.GSemestre and 
	   Target.Fecha = Source.Fecha and Target.PCodigo = Source.PCodigo and
	   Target.SCodigo = Source.SCodigo
	WHEN NOT MATCHED BY TARGET THEN
	INSERT ([FCodigo],[Correo],[CSigla],[GNumero],[GAnno],[GSemestre],[Fecha],[PCodigo], [SCodigo], [Justificacion])
	VALUES (@codFormulario, @correo, @siglaCurso, @numGrupo, @anno, @semestre, @fecha, @codPregunta, @codseccion, @justificacion);
END;