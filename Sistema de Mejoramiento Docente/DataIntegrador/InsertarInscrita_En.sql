CREATE PROCEDURE [dbo].[InsertarInscrita_En]
	@CodUnidadAc varchar(10),
	@CodCarrera varchar(10)
AS
begin
	--Esta condicion revisa que no se intente inscribir unidades academicas no existentes/carreras no existentes
	IF(@CodUnidadAc IN (SELECT Codigo FROM UnidadAcademica) AND @CodCarrera IN (SELECT Codigo FROM Carrera))
	BEGIN
		Insert into Inscrita_en (CodUnidadAc, CodCarrera)
		values (@CodUnidadAc, @CodCarrera)
	END
end