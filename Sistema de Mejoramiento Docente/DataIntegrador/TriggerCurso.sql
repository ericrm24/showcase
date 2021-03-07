CREATE TRIGGER [TriggerCurso]
	ON [dbo].[Curso]
	INSTEAD OF INSERT
	AS
	DECLARE @sigla varchar(10)
	DECLARE @nombre varchar(50)

	--Nivel de aislamiento maximo porque no podemos permitir modificaciones o nuevas inserciones mientras revisamos las condiciones
	--de insercion
	set transaction isolation level serializable;
	set implicit_transactions off;
	Begin transaction transaccionCurso;

	DECLARE cursor_curso CURSOR 
	FOR SELECT Sigla, Nombre
	FROM inserted;
	OPEN cursor_curso;
	FETCH NEXT FROM cursor_curso INTO @sigla, @nombre
	WHILE @@FETCH_STATUS = 0
		BEGIN
			IF(@sigla NOT IN (SELECT Sigla FROM Curso) and @sigla not like '')
			BEGIN
				INSERT INTO Curso (Sigla, Nombre) values(@sigla, @nombre)
			END
			FETCH NEXT FROM cursor_curso INTO @sigla, @nombre
		END
	CLOSE cursor_curso;
	DEALLOCATE cursor_curso;

	Commit Transaction transaccionCurso;
	--Volver al nivel de aislamiento por default
	set transaction isolation level read committed;