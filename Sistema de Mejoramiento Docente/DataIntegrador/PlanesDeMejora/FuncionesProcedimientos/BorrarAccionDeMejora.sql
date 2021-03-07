CREATE PROCEDURE [dbo].[BorrarAccionDeMejora]
	@codigoPlan int,
	@nombreObj varchar(50),
	@descripcion varchar(25)
AS
	UPDATE AccionDeMejora
	set borrado = 1
	where codPlan = @codigoPlan and nombreObj = @nombreObj and descripcion = @descripcion;

RETURN 0
