CREATE TRIGGER [EmpadronadoEnInsertar]
	ON [dbo].[Empadronado_en]
	INSTEAD OF INSERT
	AS
	declare @correoEst varchar(50)
	declare @codCarrera varchar(10)
	declare @codEnfas varchar(10)

	--Nivel de aislamiento maximo porque no podemos permitir modificaciones o nuevas inserciones mientras revisamos las condiciones
	--de insercion
	set transaction isolation level serializable;
	set implicit_transactions off;
	Begin transaction transaccionEmpadronado;

	DECLARE cursor_Empadronado CURSOR
	FOR SELECT CorreoEstudiante, CodCarrera, CodEnfasis
	FROM inserted;
	OPEN cursor_Empadronado;
	FETCH NEXT FROM cursor_Empadronado INTO @correoEst, @codCarrera, @codEnfas
	WHILE @@FETCH_STATUS = 0
	BEGIN
		if(not exists (select *  from Empadronado_en where CodCarrera= @codCarrera and CodEnfasis = @codEnfas and CorreoEstudiante = @correoEst) )
		begin
		insert into Empadronado_en (CorreoEstudiante,CodCarrera,CodEnfasis) values (@correoEst, @codCarrera, @codEnfas)
		end
		FETCH NEXT FROM cursor_TrabajaEn INTO @correoEst, @codCarrera, @codEnfas
	END
	CLOSE cursor_Empadronado
	DEALLOCATE cursor_Empadronado

	Commit Transaction transaccionCodigoUnidad;
	--Volver al nivel de aislamiento por default
	set transaction isolation level read committed;