CREATE PROCEDURE [dbo].[AgregarSeccion]
	@codigo varchar(8),
	@nombre nvarchar(250)
AS
-- Requiere nivel de encapsulación serializable, pues es el único que permite que inserciones no se afecten
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
BEGIN TRANSACTION AgregarSeccionTrans
-- Si existe una sección con ese código, se hace rollback.
-- Si no existe, se inserta la tupla con la nueva sección.
IF NOT EXISTS (SELECT * FROM Seccion WHERE Codigo = @codigo)
BEGIN
	INSERT INTO Seccion([Codigo],[Nombre])
	VALUES(@codigo, @nombre)
	COMMIT TRANSACTION AgregarSeccionTrans
END
ELSE
BEGIN 
	ROLLBACK TRANSACTION AgregarSeccionTrans
END
RETURN 0

