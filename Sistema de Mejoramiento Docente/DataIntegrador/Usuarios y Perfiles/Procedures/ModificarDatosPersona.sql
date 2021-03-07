CREATE PROCEDURE [dbo].[ModificarDatosPersona]
	@correo VARCHAR(50),
	@correoAlt VARCHAR(50),
	@identificacion VARCHAR(30), 
    @nombre1 VARCHAR(15), 
    @nombre2 VARCHAR(15), 
    @apellido1 VARCHAR(15), 
    @apellido2 VARCHAR(15), 
	@tipoIdentificacion VARCHAR(30),
	@nuevoCarne nchar(6)
AS
	UPDATE Persona
	SET CorreoAlt = @correoAlt, 
		Identificacion = @identificacion, 
		Nombre1 = @nombre1, 
		Nombre2 = @nombre2, 
		Apellido1 = @apellido1, 
		Apellido2 = @apellido2, 
		TipoIdentificacion = @tipoIdentificacion
	WHERE Correo = @correo

	UPDATE Estudiante 
	SET Carne = @nuevoCarne
	WHERE Correo = @correo
RETURN 0
