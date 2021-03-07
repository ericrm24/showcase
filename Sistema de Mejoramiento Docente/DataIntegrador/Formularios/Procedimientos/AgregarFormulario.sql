CREATE PROCEDURE [dbo].[AgregarFormulario]
	@codigo VARCHAR(8),
	@nombre NVARCHAR(250)
AS
	-- Requiere nivel de encapsulación serializable, pues es el único que permite que inserciones no se afecten
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	BEGIN TRANSACTION AgregarFormularioTrans

	-- Debe verificar si existe un formulario con ese código. Si existe, se hace rollback de la transacción.
	-- Si no existe, puede insertar la tupla. Debido a que está serializable, ninguna otra transacción puede insertar una tupla con el mismo código.
	IF NOT EXISTS(SELECT * FROM Formulario WHERE Codigo = @codigo)
	BEGIN
		INSERT INTO Formulario([Codigo],[Nombre])
		VALUES (@codigo, @nombre);
		COMMIT TRANSACTION AgregarFormularioTrans
	END
	ELSE
	BEGIN
		ROLLBACK TRANSACTION AgregarFormularioTrans
	END
RETURN 0
