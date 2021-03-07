CREATE TRIGGER [TriggerPertenece_a]
	ON [dbo].[Pertenece_a]
	INSTEAD OF INSERT
	AS
	declare @CodCarrera varchar(10)
	declare @SiglaCurso varchar(10)
	declare @codEnf varchar(10)

	--Nivel de aislamiento maximo porque no podemos permitir modificaciones o nuevas inserciones mientras revisamos las condiciones
	--de insercion
	set transaction isolation level serializable;
	set implicit_transactions off;
	Begin transaction transaccionPerteneceA;

	DECLARE cursor_Pertenece_a CURSOR
	FOR SELECT CodCarrera, SiglaCurso, CodEnfasis
	FROM inserted;
	OPEN cursor_Pertenece_a;
	FETCH NEXT FROM cursor_Pertenece_a INTO @CodCarrera, @SiglaCurso, @codEnf
	WHILE @@FETCH_STATUS = 0
		BEGIN
			if(not exists (select * from Pertenece_a where CodCarrera= @CodCarrera and @SiglaCurso = SiglaCurso and  CodEnfasis = @codEnf) and (@codEnf not like '' and @CodCarrera not like '' and @SiglaCurso not like ''))
			begin
				insert into Pertenece_a (CodCarrera, SiglaCurso, CodEnfasis) 
				values (@CodCarrera, @SiglaCurso, @codEnf)
			end
			FETCH NEXT FROM cursor_Pertenece_a INTO @CodCarrera, @SiglaCurso, @codEnf
		END
	CLOSE cursor_Pertenece_a
	DEALLOCATE cursor_Pertenece_a

	Commit Transaction transaccionPerteneceA;
	--Volver al nivel de aislamiento por default
	set transaction isolation level read committed;
