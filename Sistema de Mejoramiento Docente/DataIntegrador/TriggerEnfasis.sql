CREATE TRIGGER [TriggerEnfasis]
	ON [dbo].[Enfasis]
	INSTEAD OF INSERT
	AS
	DECLARE @codigo varchar(10), @codigoCarrera varchar(10), @nombre varchar(50), @permisoId int
	
	--Nivel de aislamiento maximo porque no podemos permitir modificaciones o nuevas inserciones mientras revisamos las condiciones
	--de insercion
	set transaction isolation level serializable;
	set implicit_transactions off;
	Begin transaction transaccionEnfasis;
	
	DECLARE cursor_enfasis CURSOR
	FOR SELECT Codigo, CodCarrera, Nombre
	FROM inserted;
	-- Para asignar todos los permisos en este énfasis a Superusuario
	DECLARE cursor_permiso_enfasis CURSOR FOR
	SELECT Id
	FROM Permiso;

	OPEN cursor_enfasis
	FETCH NEXT FROM cursor_enfasis INTO @codigo, @codigoCarrera, @nombre
	WHILE @@FETCH_STATUS = 0
		BEGIN
			IF(NOT EXISTS (SELECT * FROM Enfasis WHERE Codigo = @codigo and CodCarrera = @codigoCarrera) and @codigo not like '' and @codigoCarrera not like '')
			BEGIN
				INSERT INTO Enfasis(CodCarrera, Codigo, Nombre)
				VALUES (@codigoCarrera, @codigo, @nombre)

				-- Colocar cursor de permisos al inicio
				OPEN cursor_permiso_enfasis
				FETCH NEXT FROM cursor_permiso_enfasis INTO @permisoId
				-- Dar todos los permisos a Superusuario
				WHILE @@FETCH_STATUS = 0
					BEGIN
						-- No insertar duplicados
						IF NOT EXISTS (
							SELECT *
							FROM PerfilPermiso
							WHERE PermisoId = @permisoId and Perfil = 'Superusuario' and CodCarrera = @codigoCarrera and CodEnfasis = @codigo
						)
							BEGIN
								INSERT INTO PerfilPermiso (PermisoId, Perfil, CodCarrera, CodEnfasis)
								VALUES (@permisoId, 'Superusuario', @codigoCarrera, @codigo)
							END
					FETCH NEXT FROM cursor_permiso_enfasis INTO @permisoId
					END
				CLOSE cursor_permiso_enfasis
				-- Asignar a admin@mail.com el perfil de Superusuario en este énfasis
				IF NOT EXISTS (
					SELECT *
					FROM UsuarioPerfil
					WHERE Usuario = 'admin@mail.com' and Perfil = 'Superusuario' and CodCarrera = @codigoCarrera and CodEnfasis = @codigo
				)
					BEGIN
						INSERT INTO UsuarioPerfil (Usuario, Perfil, CodCarrera, CodEnfasis)
						VALUES ('admin@mail.com', 'Superusuario', @codigoCarrera, @codigo)
					END
				-- Dar configuración default a los perfiles en el énfasis
				EXEC ConfigurarPerfilesDefault @codCarrera = @codigoCarrera, @codEnfasis = @codigo
			END
			FETCH NEXT FROM cursor_enfasis INTO @codigo, @codigoCarrera, @nombre
		END
	CLOSE cursor_enfasis
	DEALLOCATE cursor_enfasis
	DEALLOCATE cursor_permiso_enfasis

	Commit Transaction transaccionEnfasis;
	--Volver al nivel de aislamiento por default
	set transaction isolation level read committed;