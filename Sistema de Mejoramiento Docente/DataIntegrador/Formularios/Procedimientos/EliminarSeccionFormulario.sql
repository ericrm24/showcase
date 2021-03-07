CREATE PROCEDURE [dbo].[EliminarSeccionFormulario]
	@FCodigo varchar(50),
	@SCodigo varchar(10)
AS
	/* Las tablas de Responde_respuesta... tienen on delete cascade, por lo que esto elimina todas las respuestas.
	Se hace de esta forma, porque por ahora solo se trabaja con un formulario por persona en un grupo. */
	delete from Formulario_tiene_seccion
	where FCodigo = @FCodigo and SCodigo = @SCodigo;
RETURN 0
