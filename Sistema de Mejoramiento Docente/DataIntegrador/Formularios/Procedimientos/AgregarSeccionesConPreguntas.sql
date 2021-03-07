CREATE PROCEDURE [dbo].[AgregarSeccionesConPreguntas]
AS
BEGIN
	DELETE FROM Opciones_de_seleccion;
	DELETE FROM Pregunta_con_opciones_de_seleccion;
	DELETE FROM Pregunta_con_opciones;
	DELETE FROM Seccion_tiene_pregunta;
	DELETE FROM Pregunta;
	DELETE FROM Seccion;

	EXEC dbo.AgregarPreguntas;

	EXEC dbo.AgregarSeccion 
		@cod = 'INFOPROF',
		@nombre = 'Sobre el profesor';

	EXEC dbo.AgregarSeccion 
		@cod = 'PERSONAL',
		@nombre = 'Sobre usted como estudiante del curso';

	INSERT INTO dbo.Seccion_tiene_pregunta(SCodigo,PCodigo,Orden) VALUES ('INFOPROF','PROFES01',0);
	INSERT INTO dbo.Seccion_tiene_pregunta(SCodigo,PCodigo,Orden) VALUES ('INFOPROF','PROFES02',1);
	INSERT INTO dbo.Seccion_tiene_pregunta(SCodigo,PCodigo,Orden) VALUES ('INFOPROF','PROFES03',2);

	INSERT INTO dbo.Seccion_tiene_pregunta(SCodigo,PCodigo,Orden) VALUES ('PERSONAL','PERSNAL1',0);
	INSERT INTO dbo.Seccion_tiene_pregunta(SCodigo,PCodigo,Orden) VALUES ('PERSONAL','PERSNAL2',1);
	INSERT INTO dbo.Seccion_tiene_pregunta(SCodigo,PCodigo,Orden) VALUES ('PERSONAL','PERSNAL3',2);
END
