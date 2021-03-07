create procedure AgregarPlan
	@nombre varchar(50),
	@fechaInicio date,
	@fechaFin date,
	@codigo int out
as
	declare @ultimo int;
	select @ultimo = MAX(codigo)
	from PlanDeMejora;

	select @codigo = @ultimo + 1;
	insert into PlanDeMejora(codigo, nombre, fechaInicio, fechaFin, borrado)
	values(@codigo, @nombre, @fechaInicio, @fechaFin, 0);
	return
