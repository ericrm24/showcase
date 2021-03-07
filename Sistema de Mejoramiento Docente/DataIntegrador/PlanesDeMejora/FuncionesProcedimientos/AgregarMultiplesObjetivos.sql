CREATE TYPE ObjetivoParametro
AS TABLE(
	codPlan int not null PRIMARY key,
	nombre varchar(50) not null,
	descripcion varchar(250),
	fechaInicio date,
	fechaFin date,
	nombTipoObj varchar(50),
	codPlantilla int,
	borrado bit
	)
GO

CREATE PROCEDURE [dbo].AgregarMultiplesObjetivos
	@Objetivos ObjetivoParametro READONLY
	AS
	declare @status int = 0;
	begin try
		begin transaction
			insert into Objetivo
			select * from @Objetivos;
		commit transaction
	end try
	begin catch
		select @status = ERROR_NUMBER()
		rollback transaction
	end catch;
RETURN @status;