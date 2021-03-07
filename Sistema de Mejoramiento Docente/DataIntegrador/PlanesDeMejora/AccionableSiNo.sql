CREATE TABLE [dbo].[AccionableSiNo]
(
	codPlan int not null,
	nombreObj varchar(50) not null,
	descripAcMej varchar(250) not null,
	descripcion varchar(250) not null,
	avance char(2) null default 'No',

	constraint PK_AccionableSiNo primary key(codPlan, nombreObj, descripAcMej, descripcion),
	constraint FK_AccionableSiNo_Accionable foreign key(codPlan, nombreObj, descripAcMej, descripcion)
		references Accionable(codPlan, nombreObj, descripAcMej, descripcion) on delete cascade on update cascade
)