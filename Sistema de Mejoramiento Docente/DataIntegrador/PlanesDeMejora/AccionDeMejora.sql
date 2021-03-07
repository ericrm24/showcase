-- Mosqueteros --

CREATE TABLE [dbo].[AccionDeMejora]
(
	codPlan int not null,
	nombreObj varchar(50) not null,
	descripcion varchar(250) not null,
	fechaInicio date,
	fechaFin date,
	codPlantilla int,
	borrado bit,
	constraint DateOrderAM check(fechaFin >= fechaInicio),
	constraint PK_AccionDeMejora primary key(codPlan, nombreObj, descripcion),
	constraint FK_AccionDeMejora_Objetivo foreign key(codPlan, nombreObj) references Objetivo(codPlan, nombre) on delete cascade,
	constraint FK_AccionDeMejora_PlantillaAccMej foreign key(codPlantilla) references PlantillaAccionDeMejora(codigo) on delete set null
)
