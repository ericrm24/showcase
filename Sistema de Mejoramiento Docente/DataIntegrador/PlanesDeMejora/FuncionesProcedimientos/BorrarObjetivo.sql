CREATE PROCEDURE [dbo].[BorrarObjetivo]
	@codigoPlan int,
	@nombreObj varchar(50)
AS
	UPDATE Objetivo
	set borrado = 1
	where codPlan = @codigoPlan and nombre = @nombreObj;

	UPDATE AccionDeMejora
	set borrado = 1
	where codPlan = @codigoPlan and nombreObj = @nombreObj;
RETURN 0
