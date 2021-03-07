CREATE PROCEDURE [dbo].[EliminarRespuestasDeFormulario]
	@codFormulario varchar(8),
	@correo varchar(50),
	@csigla varchar(10),
	@gnumero tinyint,
	@ganno int,
	@gsemestre tinyint
AS
	/* Las tablas de Responde_respuesta... tienen on delete cascade, por lo que esto elimina todas las respuestas.
	Se hace de esta forma, porque por ahora solo se trabaja con un formulario por persona en un grupo. */
	delete from Respuestas_a_formulario
	where FCodigo = @codFormulario and Correo = @correo and CSigla = @csigla and GNumero = @gnumero and GAnno = @ganno and GSemestre = @gsemestre;
RETURN 0
