CREATE TABLE [dbo].[AccionablePorcentaje]
(
	codPlan int not null,
	nombreObj varchar(50) not null,
	descripAcMej varchar(250) not null,
	descripcion varchar(250) not null,
	avance int null default 0,

	constraint PK_AccionablePorcentaje primary key(codPlan, nombreObj, descripAcMej, descripcion),
	constraint FK_AccionablePorcentaje_Accionable foreign key(codPlan, nombreObj, descripAcMej, descripcion)
		references Accionable(codPlan, nombreObj, descripAcMej, descripcion) on delete cascade on update cascade,
	constraint Avance check (avance >= 0 and avance <= 100)
)