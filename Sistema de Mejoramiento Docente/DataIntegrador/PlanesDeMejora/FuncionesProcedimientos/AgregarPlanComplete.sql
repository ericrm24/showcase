CREATE PROCEDURE [dbo].[AgregarPlanComplete]
(
	@tablaPlan					 as dbo.PlanTabla			 readonly,
	@tablaObjetivos				 as dbo.ObjetivosTabla		 readonly,
	@tablaAcciones				 as dbo.AccionDeMejoraTabla	 readonly,
	@tablaAccionables			 as dbo.AccionablesTabla 	 readonly,
	@tablaAsocPlanFormularios	 as dbo.AsocPlanFormulario   readonly,
	@tablaAsocObjetivosSecciones as dbo.AsocObjetivoSeccion  readonly,
	@tablaAsocAccionesPreguntas  as dbo.AsocAccionPregunta   readonly,
	@tablaAsocPlanProfesores     as dbo.AsocPlanProfesores   readonly
)
AS
BEGIN

BEGIN TRY
	BEGIN TRANSACTION
				
		insert into PlanDeMejora
		select * from @tablaPlan

		insert into Objetivo
		select * from @tablaObjetivos

		insert into AccionDeMejora
		select * from @tablaAcciones

		exec dbo.AgregarMultiplesAccionables @tablaAccionables
		
		insert into Evalua
		select * from @tablaAsocPlanFormularios

		insert into ObjVsSeccion
		select * from @tablaAsocObjetivosSecciones

		insert into AccionVsPregunta
		select * from @tablaAsocAccionesPreguntas


		insert into AsignadoA
		select * from @tablaAsocPlanProfesores

	COMMIT TRANSACTION
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION
END CATCH
	
END;
