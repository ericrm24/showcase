CREATE TABLE [dbo].[ObjVsSeccion]
(
	codPlanObj	int			NOT NULL,
	nombreObj	varchar(50) NOT NULL,
	codSeccion	varchar(8)  NOT NULL,
	PRIMARY KEY (codPlanObj, nombreObj, codSeccion),
	constraint FK_ObjVsSeccion_Objetivo FOREIGN KEY(codPlanObj, nombreObj) REFERENCES Objetivo(codPlan, nombre),
	constraint FK_ObjVsSeccion_Seccion  FOREIGN KEY(codSeccion) REFERENCES Seccion(Codigo)
)
