-- Mosqueteros --

CREATE TABLE [dbo].[Objetivo]
(
	codPlan int not null,
	nombre varchar(50) not null,
	descripcion varchar(250),
	fechaInicio date,
	fechaFin date,
	nombTipoObj varchar(50),
	codPlantilla int,
	borrado bit,
	constraint DateOrderObj check(fechaFin >= fechaInicio),
	constraint PK_Objetivo primary key(codPlan, nombre),
	constraint FK_Objetivo_Plan foreign key(codPlan) references PlanDeMejora(codigo) on delete cascade,
	constraint FK_Objetivo_TipoObj foreign key(nombTipoObj) references TipoObjetivo(nombre) on delete set null,
	constraint FK_Objetivo_PlantillaObjetivo foreign key(codPlantilla) references PlantillaObjetivo(codigo) on delete set null
)
