CREATE TRIGGER [TriggerFuncionario]
	ON [dbo].[Funcionario]
	INSTEAD OF INSERT
	AS
	DECLARE @correo varchar(50)

	--Nivel de aislamiento maximo porque no podemos permitir modificaciones o nuevas inserciones mientras revisamos las condiciones
	--de insercion
	set transaction isolation level serializable;
	set implicit_transactions off;
	Begin transaction transaccionFuncionario;

	DECLARE cursor_funcionario CURSOR
	FOR SELECT Correo
	FROM inserted;
	OPEN cursor_funcionario;
	FETCH NEXT FROM cursor_funcionario INTO @correo
	WHILE @@FETCH_STATUS = 0
		BEGIN
			IF(@correo NOT IN (SELECT Correo FROM Funcionario) and @correo not like '')
			BEGIN
				INSERT INTO Funcionario (Correo)
				values (@correo)
			END
			FETCH NEXT FROM cursor_funcionario INTO @correo
		END
	CLOSE cursor_funcionario
	DEALLOCATE cursor_funcionario

	Commit Transaction transaccionFuncionario;
	--Volver al nivel de aislamiento por default
	set transaction isolation level read committed;