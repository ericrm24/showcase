go
CREATE TRIGGER [dbo].[CrearUsuarioDespuesdePersona]
	ON [dbo].[Persona]
	Instead of INSERT
AS
BEGIN
	DECLARE
	@correoactual varchar(50),
	@output bit
	DECLARE
	personasAgregadas CURSOR FOR
	SELECT Correo
	FROM inserted

	OPEN personasAgregadas
	FETCH NEXT FROM personasAgregadas into @correoactual;

	while @@FETCH_STATUS=0
	begin
		insert into Persona
		select *
		from inserted
		where Correo=@correoactual

		EXEC AgregarUsuario @pLogin=@correoactual, @pPassword=@correoactual, @activo=1, @estado=@output

		FETCH NEXT FROM personasAgregadas into @correoactual;
	end
	close personasAgregadas
	deallocate personasAgregadas
end