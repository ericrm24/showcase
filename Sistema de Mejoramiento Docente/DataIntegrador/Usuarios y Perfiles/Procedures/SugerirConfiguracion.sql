/*Procedimiento para escoger el perfil en la carrera y énfasis que genere más valor al usuario al ingresar al sistema.
Es decir, la combinación de esos tres factores en el que posea más permisos para hacer cosas en el sistema.
Devuelve mediante parámetros de salida el perfil, la carrera y el énfasis en el que tiene más permisos.
El programa lo utiliza para configurar estas opciones al usuario cuando inicia sesión.*/

CREATE PROCEDURE [dbo].[SugerirConfiguracion]
	@CorreoUsuario VARCHAR(50),
	@PerfilPoderoso VARCHAR(50) OUTPUT,
	@CarreraPoderosa VARCHAR(10) OUTPUT,
	@EnfasisPoderoso VARCHAR(10) OUTPUT
AS
	DECLARE @PerfilI VARCHAR(50)
	DECLARE @CarreraI VARCHAR(10)
	DECLARE @EnfasisI VARCHAR(10)
	DECLARE @CantidadPermisos INT = 0
	DECLARE @CantidadPermisosAnterior INT = 0

	DECLARE i CURSOR
	FOR SELECT * FROM PerfilesXUsuario(@CorreoUsuario);
	
	OPEN i;
	FETCH NEXT FROM i INTO @PerfilI

	WHILE @@FETCH_STATUS = 0
	BEGIN 
		DECLARE j CURSOR
		FOR SELECT codCarrera FROM CarrerasXPerfilXUsuario(@CorreoUsuario, @PerfilI)
		OPEN j
		FETCH NEXT FROM j INTO @CarreraI
		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE k CURSOR
			FOR SELECT codEnfasis FROM EnfasisXCarreraXPerfil(@CorreoUsuario, @CarreraI, @PerfilI)

			OPEN k;
			FETCH NEXT FROM k INTO @EnfasisI

			WHILE @@FETCH_STATUS = 0
			BEGIN 

				SELECT @CantidadPermisos = COUNT(*) FROM PerfilPermiso
				WHERE Perfil = @PerfilI AND  CodCarrera = @CarreraI AND CodEnfasis = @EnfasisI

				/*Si la cantidad de permisos en esta combinación es mayor que la anterior, selecciónela.*/
				IF(@CantidadPermisos >= @CantidadPermisosAnterior)
				BEGIN
					SET @PerfilPoderoso = @PerfilI
					SET @CarreraPoderosa = @CarreraI
					SET @EnfasisPoderoso = @EnfasisI
					SET @CantidadPermisosAnterior = @CantidadPermisos	
				END

				FETCH NEXT FROM k INTO @EnfasisI

				END
				CLOSE k;
				DEALLOCATE k;

			FETCH NEXT FROM j INTO @CarreraI
			
		END

		CLOSE j;
		DEALLOCATE j;

		FETCH NEXT FROM i INTO @PerfilI
	END

	CLOSE i;
	DEALLOCATE i;