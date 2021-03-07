CREATE TRIGGER [TriggerPesoAccionable]
	ON [dbo].[Accionable]
	AFTER INSERT, UPDATE
	AS
	BEGIN
		DECLARE @pesoTotal int, @pesoActual int
			SELECT @pesoTotal = SUM(peso) 
			FROM Accionable a
			WHERE a.codPlan = inserted.codPlan AND
				  a.nombreObj = inserted.nombreObj AND
				  a.descripAcMej = inserted.descripAcMej
			GROUP BY codPlan,nombreObj,descripAcMej
		DECLARE cursor_pesos CURSOR FOR
			SELECT peso 
			FROM Accionable a
			WHERE a.codPlan = inserted.codPlan AND
				  a.nombreObj = inserted.nombreObj AND
				  a.descripAcMej = inserted.descripAcMej
		OPEN cursor_pesos
		FETCH NEXT FROM cursor_pesos INTO @pesoActual
		WHILE @@FETCH_STATUS = 0 BEGIN
			UPDATE Accionable 
			SET pesoPorcentaje = (@pesoActual * 100) / @pesoTotal
			WHERE codPlan = inserted.codPlan AND
				  nombreObj = inserted.nombreObj AND
				  descripAcMej = inserted.descripAcMej
		FETCH NEXT FROM cursor_pesos INTO @pesoActual
		END
		CLOSE cursor_pesos
		DEALLOCATE cursor_pesos
	END
