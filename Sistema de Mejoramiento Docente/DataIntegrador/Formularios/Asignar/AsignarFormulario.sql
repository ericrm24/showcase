-- Procedimiento que se encarga de asignar un formulario
-- en un periodo especifico, y a un grupo particular
CREATE PROCEDURE [dbo].[AsignarFormulario]
	@FormularioCodigo varchar(8),
	@CursoSigla varchar(10),
	@GrupoNumero tinyint,
	@GrupoAnno int,
	@GrupoSemestre tinyint,
	@FechaInicio date,
	@FechaFin	date
AS
	IF (@FechaInicio <= @FechaFin)
		BEGIN TRY
			BEGIN TRANSACTION activarFormulario
					-- Insertamos en la Tabla de Activa Por, la relación de grupo con formulario
					MERGE INTO Activa_por AS Target
					USING (VALUES
							(@FormularioCodigo, @CursoSigla, @GrupoNumero,
							@GrupoAnno, @GrupoSemestre) 
					)
					AS Source (FCodigo,CSigla, [GNumero], [GAnno], [GSemestre])
					ON	TARGET.FCodigo = Source.FCodigo AND
						TARGET.CSigla = Source.CSigla AND 
						TARGET.GNumero = Source.GNumero  AND
						TARGET.GAnno = Source.GAnno AND
						TARGET.GSemestre = Source.GSemestre
					WHEN NOT MATCHED BY TARGET THEN
					INSERT (FCodigo, CSigla, GNumero, GAnno, GSemestre)
					VALUES (@FormularioCodigo,	@CursoSigla,
							@GrupoNumero, @GrupoAnno,
							@GrupoSemestre);


					-- Insertar en periodo que hace referencia a Activa_Por
					MERGE INTO Periodo_activa_por AS Target
					USING (VALUES
							(@FormularioCodigo, @CursoSigla, @GrupoNumero,
							@GrupoAnno, @GrupoSemestre, @FechaInicio, @FechaFin) 
					)
					AS Source ( FCodigo,CSigla, [GNumero], [GAnno], [GSemestre], 
								[FechaInicio], [FechaFin])
					ON	TARGET.FCodigo = Source.FCodigo AND
						TARGET.CSigla = Source.CSigla AND 
						TARGET.GNumero = Source.GNumero  AND
						TARGET.GAnno = Source.GAnno AND
						TARGET.GSemestre = Source.GSemestre AND
						TARGET.FechaInicio = Source.FechaInicio AND
						TARGET.FechaFin = Source.FechaFin
					WHEN NOT MATCHED BY TARGET THEN
					INSERT (FCodigo, CSigla, GNumero, GAnno, GSemestre, FechaInicio, FechaFin)
					VALUES (@FormularioCodigo,	@CursoSigla,
							@GrupoNumero, @GrupoAnno,
							@GrupoSemestre, @FechaInicio, @FechaFin);
			COMMIT TRANSACTION activarFormulario
		END TRY
		BEGIN CATCH
			PRINT 'No se asigno correctamente el formulario';
			THROW;
			-- Sino logro insertar en ambas tuplas, la relacion no existe.
			ROLLBACK TRANSACTION activarFormulario;
		END CATCH
	ELSE
		PRINT 'La fecha de Fin es mayor a la fecha de inicio'
RETURN 0
