CREATE PROCEDURE [dbo].[ObtenerSeccionesAsociadasAObjetivo]
	@idPlan int,
	@nombreObj	varchar(50)
AS
BEGIN
	select DISTINCT ovs.codSeccion
	from ObjVsSeccion ovs
	where ovs.codPlanObj = @idPlan and ovs.nombreObj = @nombreObj
END