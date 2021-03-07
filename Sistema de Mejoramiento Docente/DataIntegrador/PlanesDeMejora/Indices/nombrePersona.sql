CREATE INDEX [nombrePersona]
	ON [dbo].[Persona]
	(Apellido1, Nombre1)
	INCLUDE(Apellido2, Nombre2)
