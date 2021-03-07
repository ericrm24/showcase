create procedure GetTeacherName
	@Correo varchar(50),
	@NombreCompleto varchar(50) out
as
begin
	select @NombreCompleto = concat(p.Nombre1, ' ', p.Apellido1)
	from Persona p
	where @Correo = p.Correo;
end