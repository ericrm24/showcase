CREATE TABLE [dbo].[SeAsignaA]
(
	codigoPlan int not null,
	codigoForm varchar(8) not null,

	constraint FK_SeAsignaA_PlanDeMejora foreign key(codigoPlan) references PlanDeMejora(codigo) on delete cascade,
	constraint FK_SeAsignaA_Formulario foreign key(codigoForm) references Formulario(Codigo) on delete cascade,
	constraint PK_SeAsignaA primary key(codigoPlan, codigoForm)
)