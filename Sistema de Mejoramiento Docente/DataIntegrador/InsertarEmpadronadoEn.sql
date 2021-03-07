CREATE PROCEDURE [dbo].[InsertarEmpadronadoEn]
	@CorreoEstudiante VARCHAR(50),
	@CodCarrera VARCHAR(10),
	@CodEnfasis VARCHAR(10)
AS
begin
	--Esta condicion revisa que no se intente empadronar estudiantes no existentes/enfasis no existentes
	IF(@CorreoEstudiante IN (SELECT Correo FROM Estudiante) AND EXISTS (SELECT * FROM Enfasis WHERE CodCarrera = @CodCarrera and Codigo = @CodEnfasis))
	BEGIN
		insert into Empadronado_en (CorreoEstudiante, CodEnfasis, CodCarrera)
		values (@CorreoEstudiante,@CodEnfasis,@CodCarrera)
	END
end
