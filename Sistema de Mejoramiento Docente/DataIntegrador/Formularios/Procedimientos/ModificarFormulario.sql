CREATE PROCEDURE [dbo].[ModificarFormulario]
	@codviejo varchar(8),
	@codnuevo varchar(8),
	@nombre varchar(250),
	@modificacionexitosa bit output
AS
	begin try
		begin transaction ModificarFormulario
			if @codnuevo is null
			begin
				set @modificacionexitosa = 0
			end
			else
			begin
				if not exists (select * from Formulario where Codigo = @codnuevo)
				begin
					update Formulario set Codigo = @codnuevo, Nombre = @nombre where Codigo = @codviejo;
					set @modificacionexitosa = 1
				end
				else
				begin
					if @codnuevo = @codviejo
					begin
						update Formulario set Nombre = @nombre where Codigo = @codviejo;
						set @modificacionexitosa = 1
					end
					else
					begin
						set @modificacionexitosa = 0
					end
				end
			end
		commit transaction ModificarFormulario
	end try
	begin catch
		rollback transaction ModificarFormulario
		set @modificacionexitosa = 0
	end catch
RETURN 0
