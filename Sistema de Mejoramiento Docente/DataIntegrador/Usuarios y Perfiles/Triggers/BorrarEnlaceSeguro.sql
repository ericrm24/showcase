/*Debido a que no deseamos perder informacion sobre las redirecciones internas del sitio,
creamos un trigger para evitar el borrado y, en su lugar, decrementamos el valor de "usos" del hash.
De esta manera el controlador puede saber si un link que aun no ha expirado puede ser usado aun*/

CREATE TRIGGER [BorrarEnlaceSeguro]
ON [EnlaceSeguro]
INSTEAD OF DELETE
AS
BEGIN
	DECLARE
	@hashActual varchar(64),
	@usosActual int

	--Utlizamos un cursor para poder manejar el borrado de varios links al mismo tiempo, si fuese necesario.
	DECLARE	
	EnlacesBorrados CURSOR FOR
	SELECT [Hash], [Usos]
	FROM deleted

	OPEN EnlacesBorrados
	FETCH NEXT FROM EnlacesBorrados into @hashActual, @usosActual;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--decrementamos el valor de usos del enlace que ha sido utilizado si es mayor a 0, sino no hacemos nada.
		IF (@usosActual > 0)
		BEGIN			
			update EnlaceSeguro set Usos = Usos-1
			WHERE [Hash] = @hashActual
		END
		FETCH NEXT FROM EnlacesBorrados into @hashActual, @usosActual;
	END
	CLOSE EnlacesBorrados
	DEALLOCATE EnlacesBorrados
end