CREATE TABLE [dbo].[AccionVsPregunta]
(
	codPlanADM	INT,
	nombreObj	varchar(50),
	descripcion varchar(250),
	codPregunta varchar(8),
	PRIMARY KEY(codPlanADM, nombreObj, descripcion, codPregunta),
	CONSTRAINT FK_AccionVsPregunta_AccionDeMejora	FOREIGN KEY(codPlanADM, nombreObj, descripcion) REFERENCES AccionDeMejora(codPlan, nombreObj, descripcion),
	CONSTRAINT FK_AccionVsPregunta_Pregunta			FOREIGN KEY(codPregunta) REFERENCES Pregunta(Codigo),
)
