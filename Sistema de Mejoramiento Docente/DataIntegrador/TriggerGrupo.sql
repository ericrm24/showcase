CREATE TRIGGER [TriggerGrupo]
	ON [dbo].[Grupo]
	INSTEAD OF INSERT
	AS
	DECLARE @sigla varchar(10), @num tinyint, @semestre tinyint, @anno int

	--Nivel de aislamiento maximo porque no podemos permitir modificaciones o nuevas inserciones mientras revisamos las condiciones
	--de insercion
	set transaction isolation level serializable;
	set implicit_transactions off;
	Begin transaction transaccionGrupo;

	DECLARE cursor_grupo CURSOR
	FOR SELECT SiglaCurso, NumGrupo, Semestre, Anno
	FROM inserted;
	OPEN cursor_grupo;
	FETCH NEXT FROM cursor_grupo INTO @sigla, @num, @semestre, @anno
	WHILE @@FETCH_STATUS = 0
		BEGIN
			IF(NOT EXISTS (SELECT * FROM Grupo WHERE SiglaCurso = @sigla and NumGrupo = @num and Semestre = @semestre and Anno = @anno) and @sigla not like '' and @num not like '' and @semestre not like '' and @anno not like '')
			BEGIN
				INSERT INTO Grupo (SiglaCurso,NumGrupo,Semestre,Anno)
				values(@sigla, @num, @semestre, @anno)
			END
			FETCH NEXT FROM cursor_grupo INTO @sigla, @num, @semestre, @anno
		END
	CLOSE cursor_grupo
	DEALLOCATE cursor_grupo

	Commit Transaction transaccionGrupo;
	--Volver al nivel de aislamiento por default
	set transaction isolation level read committed;