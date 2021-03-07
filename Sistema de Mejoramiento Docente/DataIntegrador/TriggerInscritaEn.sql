CREATE TRIGGER [TriggerInscritaEn] 
	ON [dbo].[Inscrita_en]
	INSTEAD OF INSERT
	AS
	declare @CodUnidad varchar(10)
	declare @CodCarrera varchar(10)

	
	--Nivel de aislamiento maximo porque no podemos permitir modificaciones o nuevas inserciones mientras revisamos las condiciones
	--de insercion
	set transaction isolation level serializable;
	set implicit_transactions off;
	Begin transaction transaccionInscritaEn;

	DECLARE cursor_inscritaEn CURSOR
	FOR SELECT CodUnidadAc, CodCarrera
	FROM inserted;
	OPEN cursor_inscritaEn;
	FETCH NEXT FROM cursor_inscritaEn INTO @CodUnidad, @CodCarrera
	WHILE @@FETCH_STATUS = 0
		BEGIN
			if(not exists (select * from Inscrita_en where CodUnidadAc= @CodUnidad and CodCarrera =@CodCarrera) and @CodUnidad not like '' and @CodCarrera not like '')
			begin
				insert into Inscrita_en (CodUnidadAc, CodCarrera)
				values (@CodUnidad, @CodCarrera)
			end
			FETCH NEXT FROM cursor_inscritaEn INTO @CodUnidad, @CodCarrera
		END
	CLOSE cursor_inscritaEn
	DEALLOCATE cursor_inscritaEn

	Commit Transaction transaccionInscritaEn;
	--Volver al nivel de aislamiento por default
	set transaction isolation level read committed;