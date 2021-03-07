CREATE TRIGGER [TriggerPeriodo]
ON [dbo].[Periodo_activa_por]
INSTEAD OF INSERT
AS
BEGIN
	
	IF EXISTS(
			SELECT * 
			FROM Periodo_activa_por p, inserted i
			WHERE p.CSigla = i.CSigla 
			AND p.FCodigo = i.FCodigo 
			AND p.GAnno = i.GAnno
			AND p.GNumero = i.GNumero
			AND p.GSemestre = i.GSemestre
			AND p.FechaInicio BETWEEN i.FechaInicio AND i.FechaFin
			OR p.FechaFin BETWEEN i.FechaInicio AND i.FechaFin
	)
	BEGIN 
		UPDATE Periodo_activa_por
		SET FechaInicio = inserted.FechaInicio
		FROM inserted
		WHERE Periodo_activa_por.CSigla = inserted.CSigla 
		AND Periodo_activa_por.FCodigo = inserted.FCodigo 
		AND Periodo_activa_por.GAnno = inserted.GAnno
		AND Periodo_activa_por.GNumero = inserted.GNumero
		AND Periodo_activa_por.GSemestre = inserted.GSemestre
		AND Periodo_activa_por.FechaInicio BETWEEN inserted.FechaInicio AND inserted.FechaFin
		
		UPDATE Periodo_activa_por
		SET FechaFin = inserted.FechaFin
		FROM inserted
		WHERE Periodo_activa_por.CSigla = inserted.CSigla 
		AND Periodo_activa_por.FCodigo = inserted.FCodigo 
		AND Periodo_activa_por.GAnno = inserted.GAnno
		AND Periodo_activa_por.GNumero = inserted.GNumero
		AND Periodo_activa_por.GSemestre = inserted.GSemestre
		AND Periodo_activa_por.FechaFin BETWEEN inserted.FechaInicio AND inserted.FechaFin
	END
	ELSE
	BEGIN
		INSERT INTO Periodo_activa_por
		SELECT * FROM inserted
	END
END
