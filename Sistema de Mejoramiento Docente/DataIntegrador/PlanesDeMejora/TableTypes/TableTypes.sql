CREATE TYPE dbo.PlanTabla AS TABLE 
(
	codigo		int,
	nombre		varchar(50),
	fechaInicio date,
	fechaFin	date,
	borrado		bit
)
GO

CREATE TYPE dbo.ObjetivosTabla AS TABLE 
(
	codigoPlan		int,
	nombre			varchar(50),
	descripcion		varchar(250),
	fechaInicio		date,
	fechaFin		date,
	nombreTipoObj	varchar(50),
	codPlantilla	int,
	borrado			bit
)
GO

CREATE TYPE dbo.AccionDeMejoraTabla AS TABLE 
(
	codigoPlan		int,
	nombreObj		varchar(50),
	descripcion		varchar(250),
	fechaInicio		date,
	fechaFin		date,
	codPlantilla	int,
	borrado			bit
)
GO

CREATE TYPE dbo.AccionableTabla AS TABLE 
(
	codigoPlan			int,
	nombreObj			varchar(50),
	descripcionAccion	varchar(250),
	descripcion			varchar(250),
	fechaInicio			date,
	fechaFin			date,
	tipo				char,
	peso				int,
	pesoPorcentaje		int
)
GO

CREATE TYPE dbo.AsocPlanFormulario AS TABLE 
(
	codigoPlan int,
	codigoForm varchar(8)
)
GO

CREATE TYPE dbo.AsocObjetivoSeccion AS TABLE 
(
	codigoPlan		int,
	nombreObjetivo	varchar(50),
	codigoSeccion	varchar(8)
)
GO

CREATE TYPE dbo.AsocAccionPregunta AS TABLE 
(
	codigoPlan			int,
	nombreObjetivo		varchar(50),
	descripcionAccion	varchar(250),
	codigoPregunta		varchar(8)
)
GO

CREATE TYPE dbo.AsocPlanProfesores AS TABLE 
(
	codigoPlan	int,
	corrProf	varchar(50)
)
GO