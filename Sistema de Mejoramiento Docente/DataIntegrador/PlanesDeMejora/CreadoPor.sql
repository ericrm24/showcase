-- Mosqueteros --

CREATE TABLE [dbo].[CreadoPor]
(
	codPlan int not null,
	corrFunc varchar(50) not null,
	
	constraint PK_CreadoPor primary key(codPlan, corrFunc),
	constraint FK_CreadoPor_PlanDeMejora foreign key(codPlan) references PlanDeMejora(codigo) on delete cascade,
	constraint FK_CreadoPor_Funcionario foreign key(corrFunc) references Funcionario(Correo) on delete cascade
)
