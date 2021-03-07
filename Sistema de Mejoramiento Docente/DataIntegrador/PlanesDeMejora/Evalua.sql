-- Mosqueteros --

CREATE TABLE [dbo].[Evalua]
(
	[codPlan] INT NOT NULL
		constraint FK_Evalua_Plan_Por_Formulario
			references PlanDeMejora(codigo) on delete cascade, 
	[codFormulario] VARCHAR(8) NOT NULL
		constraint FK_Evalua_Formulario_A_Plan
			references Formulario(Codigo) on delete cascade,
	constraint PK_Evalua
		primary key(codPlan, codFormulario)
)
