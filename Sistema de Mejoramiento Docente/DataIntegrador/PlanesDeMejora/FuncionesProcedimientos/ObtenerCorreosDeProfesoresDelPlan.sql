CREATE PROCEDURE [dbo].[ObtenerCorreosDeProfesoresDelPlan]
	@idPlan int
AS
BEGIN
	select DISTINCT a.corrProf
	from AsignadoA a
	where a.codPlan = @idPlan
END