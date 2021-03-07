CREATE PROCEDURE [dbo].[GuardarRespuestaAFormulario]
	@codFormulario varchar(8),
	@correo varchar(50),
	@siglaCurso varchar(6),
	@numGrupo tinyint,
	@anno int,
	@semestre tinyint,
	@fecha date,
	@finalizado bit

AS
BEGIN
	-- Se crea la tupla que indica que hubo una respuesta a un formulario, por parte de un usuario, en un grupo y día dado
	-- Es necesario que se ejecute este procedimiento antes de poder agregar una respuesta a cualquier pregunta, ya que esas dependen de esta.
	MERGE INTO Respuestas_a_formulario AS Target
	USING (VALUES
		(@codFormulario, @correo, @siglaCurso, @numGrupo, @anno, @semestre, @fecha, @finalizado)
	)
	AS SOURCE ([FCodigo], [Correo],[CSigla],[GNumero],[GAnno],[GSemestre],[Fecha], [Finalizado])
	ON Target.FCodigo = Source.FCodigo and Target.Correo = Source.Correo and Target.CSigla = Source.CSigla 
		and Target.GNumero = Source.GNumero and Target.GAnno = Source.GAnno 
		and Target.GSemestre = Source.GSemestre and Target.Fecha = Source.Fecha
	WHEN NOT MATCHED BY TARGET THEN
	INSERT (FCodigo, Correo, CSigla, GNumero, GAnno, GSemestre, Fecha, Finalizado)
	VALUES (@codFormulario, @correo, @siglaCurso, @numGrupo, @anno, @semestre, @fecha, @finalizado);
END