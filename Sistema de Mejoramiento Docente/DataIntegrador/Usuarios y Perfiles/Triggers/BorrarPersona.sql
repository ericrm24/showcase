create trigger [BorrarPersona]
on [Persona]
instead of delete
as
begin
	DECLARE
	@correoactual varchar(50)
	DECLARE
	PersonasBorradas CURSOR FOR
	SELECT Correo
	FROM deleted

	OPEN PersonasBorradas
	FETCH NEXT FROM PersonasBorradas into @correoactual;
	while @@FETCH_STATUS = 0
	begin
		update Usuario set Activo = 0
		where Username = @correoactual

		update Persona set Borrado = 1
		where Correo = @correoactual
		FETCH NEXT FROM PersonasBorradas into @correoactual;
	end
	close PersonasBorradas
	deallocate PersonasBorradas
end
