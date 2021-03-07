CREATE PROCEDURE ObtenerSeccionesDeFormulario @CodForm varchar(8)
AS
	SELECT s.Codigo, s.Nombre, t.Orden
	FROM (Seccion s JOIN Formulario_tiene_seccion t ON s.Codigo = t.SCodigo) JOIN Formulario f ON t.FCodigo = f.Codigo
	WHERE f.Codigo = @CodForm
	ORDER BY t.Orden ASC
GO;
