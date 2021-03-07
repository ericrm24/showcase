CREATE PROCEDURE [dbo].[GuardarOpcionesSeleccionadas]
	@codFormulario varchar(8),
	@correo varchar(50),
	@siglaCurso varchar(6),
	@numGrupo tinyint,
	@anno int,
	@semestre tinyint,
	@fecha date,
	@codPregunta varchar(8),
	@codseccion varchar(8),
	@opcionSeleccionada tinyint
AS
BEGIN
	MERGE INTO  Opciones_seleccionadas_respuesta_con_opciones  AS Target
	USING (VALUES
		(@codFormulario, @correo, @siglaCurso, @numGrupo, @anno, @semestre, @fecha, @codPregunta, @codseccion, @opcionSeleccionada)
	)
	AS SOURCE ([FCodigo],[Correo],[CSigla],[GNumero],[GAnno],[GSemestre],[Fecha],[PCodigo], [SCodigo], [OpcionSeleccionada])
	ON Target.FCodigo = Source.FCodigo and Target.Correo = Source.Correo and 
	   Target.CSigla  = Source.CSigla  and Target.GNumero = Source.GNumero and 
	   Target.GAnno = Source.GAnno and Target.GSemestre = Source.GSemestre and 
	   Target.Fecha = Source.Fecha and Target.PCodigo = Source.PCodigo and
	   Target.SCodigo = Source.SCodigo and Target.OpcionSeleccionada = Source.OpcionSeleccionada
	WHEN NOT MATCHED BY TARGET THEN
	INSERT ([FCodigo],[Correo],[CSigla],[GNumero],[GAnno],[GSemestre],[Fecha],[PCodigo], [SCodigo], [OpcionSeleccionada])
	VALUES (@codFormulario, @correo, @siglaCurso, @numGrupo, @anno, @semestre, @fecha, @codPregunta, @codseccion, @opcionSeleccionada);
END
