CREATE PROCEDURE [dbo].[ObtenerFormulariosAsociados]
	@idPlan int
AS
BEGIN
	select DISTINCT e.codFormulario
	from Evalua e
	where e.codPlan = @idPlan
END
