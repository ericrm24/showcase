CREATE PROCEDURE [dbo].[BorrarPlan]
	@codigoPlan int
AS
	UPDATE PlanDeMejora
	set borrado = 1
	where codigo = @codigoPlan;

	UPDATE Objetivo
	set borrado = 1
	where codPlan = @codigoPlan;

	UPDATE AccionDeMejora
	set borrado = 1
	where codPlan = @codigoPlan;

RETURN 0
