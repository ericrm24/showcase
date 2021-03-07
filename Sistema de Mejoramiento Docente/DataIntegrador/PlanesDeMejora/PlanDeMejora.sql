-- Mosqueteros --

CREATE TABLE [dbo].[PlanDeMejora]
(
	codigo int not null,
	nombre varchar(50),
	fechaInicio date,
	fechaFin date,
	borrado bit,
	constraint DateOrderPM check(fechaFin >= fechaInicio),
	constraint PK_PlanDeMejora primary key(codigo)
)
