-- Mosqueteros --

CREATE TABLE [dbo].[Responsable_De]
(
	codPlan int not null,
	nombreObj varchar(50) not null,
	descripAcMej varchar(250) not null,
	descripAcci varchar(250) not null,
	corrFunc varchar(50) not null,

	constraint PK_ResponsableDe primary key(codPlan, nombreObj, descripAcMej, descripAcci, corrFunc),
	constraint FK_ResponsableDe_Accionable foreign key(codPlan, nombreObj, descripAcMej, descripAcci)
		references Accionable(codPlan, nombreObj, descripAcMej, descripcion) on delete cascade,
	constraint FK_ResponsableDe_Funcionario foreign key(corrFunc) references Funcionario(Correo) on delete cascade
)
