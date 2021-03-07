CREATE TRIGGER [TriggerEstudiante]
	ON [dbo].[Estudiante]
	INSTEAD OF INSERT
	AS
	DECLARE @correo varchar(50)
	DECLARE @carne nchar(6)

	--Nivel de aislamiento maximo porque no podemos permitir modificaciones o nuevas inserciones mientras revisamos las condiciones
	--de insercion
	set transaction isolation level serializable;
	set implicit_transactions off;
	Begin transaction TransaccionEstudiante;

	DECLARE cursor_estudiante CURSOR
	FOR SELECT Correo, Carne
	FROM inserted;
	OPEN cursor_estudiante;
	FETCH NEXT FROM cursor_estudiante INTO @correo, @carne
	WHILE @@FETCH_STATUS = 0
		BEGIN
			IF(@correo NOT IN (SELECT Correo FROM Estudiante) and @correo not like '')
			BEGIN
				INSERT INTO Estudiante (Correo, Carne)
				values (@correo, @carne)
			END
			FETCH NEXT FROM cursor_estudiante INTO @correo, @carne
		END
	CLOSE cursor_estudiante
	DEALLOCATE cursor_estudiante

	Commit Transaction TransaccionEstudiante;
	--Volver al nivel de aislamiento por default
	set transaction isolation level read committed;