CREATE NONCLUSTERED INDEX indice_Identificacion
	ON Persona(Identificacion) INCLUDE (TipoIdentificacion);
