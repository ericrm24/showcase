CREATE TRIGGER [TriggerCarrera]
	ON [dbo].[Carrera]
	INSTEAD OF INSERT
	AS
	DECLARE @codigo varchar(10)
	DECLARE @nombre varchar(50)

	--Nivel de aislamiento maximo porque no podemos permitir modificaciones o nuevas inserciones mientras revisamos las condiciones
	--de insercion
	set transaction isolation level serializable;
	set implicit_transactions off;
	Begin transaction transaccionCarrera;

	DECLARE cursor_carrera CURSOR
	FOR SELECT Codigo, Nombre
	FROM inserted;
	OPEN cursor_carrera;
	FETCH NEXT FROM cursor_carrera INTO @codigo, @nombre
	WHILE @@FETCH_STATUS = 0
		BEGIN
			IF(@codigo NOT IN (SELECT Codigo FROM Carrera) and @codigo not like '')
			BEGIN
				INSERT INTO Carrera (Codigo, Nombre)
				values (@codigo, @nombre)
			END
			FETCH NEXT FROM cursor_carrera INTO @codigo, @nombre
		END
	CLOSE cursor_carrera;
	DEALLOCATE cursor_carrera;
	
	Commit Transaction transaccionCarrera;
	--Volver al nivel de aislamiento por default
	set transaction isolation level read committed;