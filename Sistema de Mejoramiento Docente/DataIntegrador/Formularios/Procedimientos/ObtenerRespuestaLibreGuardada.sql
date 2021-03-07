CREATE FUNCTION [dbo].[ObtenerRespuestaLibreGuardada]
(
	@FCodigo varchar(8),
	@Correo varchar(50),
	@CSigla varchar(10),
	@GNumero tinyint,
	@GAnno int,
	@GSemestre tinyint,
	@PCodigo varchar(8),
	@SCodigo varchar(8)
)
RETURNS TABLE
AS
	RETURN
		select *
		from Responde_respuesta_libre
		where FCodigo = @FCodigo and Correo = @Correo and CSigla = @CSigla and GNumero = @GNumero and
		GAnno = @GAnno and GSemestre = @GSemestre and PCodigo = @PCodigo and SCodigo = @SCodigo;
