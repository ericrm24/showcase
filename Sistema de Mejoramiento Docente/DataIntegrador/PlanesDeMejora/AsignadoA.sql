CREATE TABLE [dbo].[AsignadoA]
(
	codPlan int not null,
	corrProf varchar(50) not null,
	constraint PK_AsignadoA primary key(codPlan, corrProf),
	constraint FK_AsignadoA_PlanDeMejora foreign key(codPlan) references PlanDeMejora(codigo) on delete cascade,
	constraint FK_AsignadoA_Profesor foreign key(corrProf) references Profesor(Correo) on delete cascade
)
