CREATE TABLE [dbo].[AccionableEscala]
(
	codPlan int not null,
	nombreObj varchar(50) not null,
	descripAcMej varchar(250) not null,
	descripcion varchar(250) not null,
	valorMinimo int not null default 0,
	valorMaximo int not null default 10,
	avance int null default 0,

	constraint PK_AccionableEscala primary key(codPlan, nombreObj, descripAcMej, descripcion),
	constraint FK_AccionableEscala_Accionable foreign key(codPlan, nombreObj, descripAcMej, descripcion)
		references Accionable(codPlan, nombreObj, descripAcMej, descripcion) on delete cascade on update cascade,
	constraint ValorMin check (valorMinimo >= 0),
	constraint ValorMax check (valorMaximo >= 1),
	constraint ValoresMinMax check (valorMinimo < valorMaximo)
)