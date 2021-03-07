CREATE TRIGGER [TriggerEmpadronadoEn]
ON [dbo].[Empadronado_en]
	INSTEAD OF INSERT
	AS
	declare @Correo varchar(50)
	declare @CodCarrera varchar(10)
	declare @CodEnfasis varchar(10)


	--Nivel de aislamiento maximo porque no podemos permitir modificaciones o nuevas inserciones mientras revisamos las condiciones
	--de insercion
	set transaction isolation level serializable;
	set implicit_transactions off;
	Begin transaction transaccionEmpadronadoEn;

	DECLARE cursorEmpadronado CURSOR
	FOR SELECT CorreoEstudiante, CodCarrera, CodEnfasis
	FROM inserted;
	OPEN cursorEmpadronado;
	FETCH NEXT FROM cursorEmpadronado INTO @Correo,@CodCarrera,@CodEnfasis
	WHILE @@FETCH_STATUS = 0
		BEGIN
			if(not exists (select * from Empadronado_en where CorreoEstudiante=@Correo and CodCarrera = @CodCarrera and CodEnfasis = @CodEnfasis) and (@CodCarrera not like '' and @Correo not like '' and @CodEnfasis not like ''))
			begin
				insert into Empadronado_en select * from inserted
			end
			FETCH NEXT FROM cursor_TrabajaEn INTO @Correo,@CodCarrera,@CodEnfasis
		END
	CLOSE cursor_TrabajaEn
	DEALLOCATE cursor_TrabajaEn

	Commit Transaction transaccionEmpadronadoEn;
	--Volver al nivel de aislamiento por default
	set transaction isolation level read committed;