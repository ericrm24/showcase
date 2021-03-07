CREATE PROCEDURE [dbo].[InsertarPertenece_a]
	@CodCarrera VARCHAR(10),
	@CodEnfasis VARCHAR(10),
	@SiglaCurso VARCHAR(10)
AS
begin
	--Esta condicion revisa que no se intente insertar cursos no existentes/carreras no existentes
	IF(@SiglaCurso IN (SELECT Sigla FROM Curso) AND EXISTS (SELECT * FROM Enfasis WHERE CodCarrera = @CodCarrera and Codigo = @CodEnfasis))
	BEGIN
		insert into Pertenece_a (CodCarrera, CodEnfasis, SiglaCurso)
		values (@CodCarrera, @CodEnfasis, @SiglaCurso)
	END
end
