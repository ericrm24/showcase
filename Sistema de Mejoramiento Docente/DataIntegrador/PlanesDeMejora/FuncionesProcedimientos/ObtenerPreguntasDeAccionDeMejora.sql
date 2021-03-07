CREATE PROCEDURE [dbo].[ObtenerPreguntasDeAccionDeMejora]
	@idPlan int,
	@nombreObj	varchar(50),
	@descripcion varchar(250)
AS
BEGIN
	select DISTINCT avp.codPregunta
	from AccionVsPregunta avp
	where avp.codPlanADM = @idPlan and avp.nombreObj = @nombreObj and avp.descripcion = @descripcion
END