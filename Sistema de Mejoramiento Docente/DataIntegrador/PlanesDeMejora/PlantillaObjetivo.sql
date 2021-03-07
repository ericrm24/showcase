-- Mosqueteros --

CREATE TABLE [dbo].[PlantillaObjetivo]
(
	codigo int not null,
	nombre varchar(50),
	descripcion varchar(250),
	nombTipoObj varchar(50),

	constraint PK_PlantillaObjetivo primary key(codigo),
	constraint FK_PlantillaObj_TipoObj foreign key(nombTipoObj) references TipoObjetivo(nombre)
)
