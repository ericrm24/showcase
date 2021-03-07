CREATE TYPE AccionablesTabla
AS TABLE(
	codPlan int,
	nombreObj varchar(50),
	descripAcMej varchar(250),
	descripcion varchar(250),
	fechaInicio date,
	fechaFin date,
	tipo char,
	peso int,
	pesoPorcentaje int
	)

GO
CREATE PROCEDURE [dbo].[AgregarMultiplesAccionables]
	@Acciones AccionablesTabla READONLY
AS
	DECLARE @STATE INT = 0
	DECLARE @codigo int
	DECLARE @nombre varchar(50)
	DECLARE @descripADM varchar(250)
	DECLARE @descAcc varchar(250)
	DECLARE @inicio date
	DECLARE @fin date
	DECLARE @tipoAcc char
	DECLARE @peso int
	DECLARE @pesoPorcentaje int

	DECLARE Cursor_InsAccionable CURSOR FOR
		select a.codPlan, a.nombreObj, a.descripAcMej, a.descripcion, a.fechaInicio, a.fechaFin, a.tipo, a.peso, a.pesoPorcentaje from @Acciones a
	
	OPEN Cursor_InsAccionable

			fetch next from Cursor_InsAccionable into @codigo, @nombre, @descripADM, @descAcc, @inicio , @fin , @tipoAcc, @peso, @pesoPorcentaje
			while @@FETCH_STATUS=0 begin
				
				INSERT INTO Accionable(codPlan, nombreObj, descripAcMej, descripcion, fechaInicio, fechaFin, tipo, peso, pesoPorcentaje)
				VALUES (@codigo, @nombre, @descripADM, @descAcc, @inicio, @fin, @tipoAcc, @peso, @pesoPorcentaje)

				if @tipoAcc = 'S'
					begin
					INSERT INTO AccionableSiNo(codPlan, nombreObj, descripAcMej, descripcion)
					VALUES (@codigo, @nombre, @descripADM, @descAcc)
					end
				else if @tipoAcc = 'E'
					begin
					INSERT INTO AccionableEscala(codPlan, nombreObj, descripAcMej, descripcion, valorMaximo, valorMinimo)
					VALUES (@codigo, @nombre, @descripADM, @descAcc, 10, 0)
					end
				else if @tipoAcc = 'P'
					begin
					INSERT INTO AccionablePorcentaje(codPlan, nombreObj, descripAcMej, descripcion)
					VALUES (@codigo, @nombre, @descripADM, @descAcc)
					print('caso3')
					end

				fetch next from Cursor_InsAccionable into @codigo, @nombre, @descripADM, @descAcc, @inicio , @fin , @tipoAcc, @peso, @pesoPorcentaje
			end
	
	close Cursor_InsAccionable
	deallocate Cursor_InsAccionable
RETURN @STATE