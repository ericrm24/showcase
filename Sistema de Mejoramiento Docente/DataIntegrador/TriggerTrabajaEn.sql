CREATE TRIGGER [TriggerTrabajaEn]
	ON [dbo].[Trabaja_en]
	INSTEAD OF INSERT
	AS
	declare @Correo varchar(50)
	declare @CodUnidad varchar(10)

	--Nivel de aislamiento maximo porque no podemos permitir modificaciones o nuevas inserciones mientras revisamos las condiciones
	--de insercion
	set transaction isolation level serializable;
	set implicit_transactions off;
	Begin transaction transaccionTrabajaEn;

	DECLARE cursor_TrabajaEn CURSOR
	FOR SELECT CorreoFuncionario, CodUnidadAcademica
	FROM inserted;
	OPEN cursor_TrabajaEn;
	FETCH NEXT FROM cursor_TrabajaEn INTO @Correo, @CodUnidad
	WHILE @@FETCH_STATUS = 0
		BEGIN
			if(not exists (select * from Trabaja_en where CodUnidadAcademica=@CodUnidad and CorreoFuncionario = @Correo) and (@CodUnidad not like '' and @Correo not like ''))
			begin
				insert into Trabaja_en (CorreoFuncionario,CodUnidadAcademica)
				values (@Correo, @CodUnidad)
			end
			FETCH NEXT FROM cursor_TrabajaEn INTO @Correo, @CodUnidad
		END
	CLOSE cursor_TrabajaEn
	DEALLOCATE cursor_TrabajaEn

	Commit Transaction transaccionTrabajaEn;
	--Volver al nivel de aislamiento por default
	set transaction isolation level read committed;