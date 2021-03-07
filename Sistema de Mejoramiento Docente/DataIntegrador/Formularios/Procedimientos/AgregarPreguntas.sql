CREATE PROCEDURE [dbo].[AgregarPreguntas]
AS
BEGIN
	EXEC [dbo].[AgregarPreguntaConOpcion]
		@cod = 'PERSNAL1',
		@type = 'U',
		@enunciado = '¿Cuántas horas le dedica a este curso a la semana?';

	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL1', @orden = 0, @texto = 'Menos de 2';
	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL1', @orden = 1, @texto = 'Entre 2 y 4';
	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL1', @orden = 2, @texto = 'Entre 4 y 6';
	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL1', @orden = 3, @texto = 'Más de 6';

	EXEC [dbo].[AgregarPreguntaConOpcion]
		@cod = 'PERSNAL2',
		@type = 'U',
		@enunciado = '¿Se prepara debidamente para las clases antes de la lección?';

	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL2', @orden = 0, @texto = 'Nunca';
	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL2', @orden = 1, @texto = 'Casi nunca';
	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL2', @orden = 2, @texto = 'A veces';
	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL2', @orden = 3, @texto = 'Casi siempre';
	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL2', @orden = 4, @texto = 'Siempre';

	EXEC [dbo].[AgregarPreguntaConOpcion]
		@cod = 'PERSNAL3',
		@type = 'U',
		@enunciado = '¿Asiste regularmente a lecciones?';

	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL3', @orden = 0, @texto = 'Nunca';
	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL3', @orden = 1, @texto = 'Casi nunca';
	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL3', @orden = 2, @texto = 'A veces';
	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL3', @orden = 3, @texto = 'Casi siempre';
	EXEC [dbo].[AgregarOpcion] @cod = 'PERSNAL3', @orden = 4, @texto = 'Siempre';

	EXEC [dbo].[AgregarPreguntaConOpcion]
		@cod = 'PROFES01',
		@type = 'U',
		@enunciado = '¿El profesor da las lecciones en el horario establecido?';

	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES01', @orden = 0, @texto = 'Nunca';
	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES01', @orden = 1, @texto = 'Casi nunca';
	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES01', @orden = 2, @texto = 'A veces';
	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES01', @orden = 3, @texto = 'Casi siempre';
	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES01', @orden = 4, @texto = 'Siempre';

	EXEC [dbo].[AgregarPreguntaConOpcion]
		@cod = 'PROFES02',
		@type = 'U',
		@enunciado = '¿El profesor trata temas relacionados con la realidad nacional cuando la materia lo permite?';

	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES02', @orden = 0, @texto = 'No sabe';
	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES02', @orden = 1, @texto = 'A veces';
	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES02', @orden = 2, @texto = 'Sí';
	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES02', @orden = 3, @texto = 'No aplica';
	

	EXEC [dbo].[AgregarPreguntaConOpcion]
		@cod = 'PROFES03',
		@type = 'U',
		@enunciado = '¿El profesor ha sido respetuoso hacia su persona?';

	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES03', @orden = 0, @texto = 'No mucho';
	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES03', @orden = 1, @texto = 'Un poco';
	EXEC [dbo].[AgregarOpcion] @cod = 'PROFES03', @orden = 2, @texto = 'Mucho';

	-- Se agregan las preguntas para el formulario de prueba del sprint 2 

	-- Pregunta 1: Sí/No/No sabe
	EXEC [dbo].[AgregarPreguntaConOpcion] -- Revisar esto. Hay que ver si se agrega una pregunta de Sí/no igual que una con opciones
		@cod = 'CI0128P1',
		@type = 'S',
		@enunciado = '¿Considera que los cursos de Bases de Datos e Ingeniería de Software se complementan bien?';

	-- Pregunta 2: Selección única
	EXEC [dbo].[AgregarPreguntaConOpcion] -- Revisar esto. Hay que ver si se agrega una pregunta de Sí/no igual que una con opciones
		@cod = 'CI0128P2',
		@type = 'U',
		@enunciado = '¿Qué tan útil considera que es tener un profesor que haga el papel de ''PO'' dentro del curso?';

	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P2', @orden = 0, @texto = 'No mucho';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P2', @orden = 1, @texto = 'Un poco';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P2', @orden = 2, @texto = 'Mucho';

	-- Pregunta 3: Selección múltiple
	EXEC [dbo].[AgregarPreguntaConOpcion] -- Revisar esto. Hay que ver si se agrega una pregunta de Sí/no igual que una con opciones
		@cod = 'CI0128P3',
		@type = 'M',
		@enunciado = '¿Cuáles de los siguientes temas ha puesto en práctica en el curso?';

	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P3', @orden = 0, @texto = 'Git';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P3', @orden = 1, @texto = 'VS';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P3', @orden = 2, @texto = 'Trabajo en equipo';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P3', @orden = 3, @texto = 'Esquemas relacionales';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P3', @orden = 4, @texto = 'Scrum';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P3', @orden = 5, @texto = 'Testing';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P3', @orden = 6, @texto = 'Negociación con el PO';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P3', @orden = 7, @texto = 'Tecnologías web';

	-- Pregunta 4, 5 y 8: Preguntas de respuesta libre
	-- Estas se hacen directamente en este procedimiento ya que no es necesario agregarles opciones 
	-- o realizar inserciones a varias tablas como las preguntas de selección
	MERGE INTO Pregunta AS Target
	USING (VALUES 
		('CI0128P4', '¿Cuál es su opinión general del curso?', 'L'),
		('CI0128P5', '¿Cuál es su opinión sobre el/la profesor(a)?', 'L'),
		('CI0128P8', '¿Cuál es su opinión del profesor en su papel de ''PO''?', 'L')
	)
	As Source ([Codigo],[Enunciado], [Tipo])
	ON Target.Codigo = Source.Codigo
	WHEN NOT MATCHED BY TARGET THEN 
	INSERT (Codigo, Enunciado, Tipo)
	VALUES (Codigo,Enunciado, Tipo);

	-- Pregunta 6: Para la sección de profesores
	EXEC [dbo].[AgregarPreguntaConOpcion] 
		@cod = 'CI0128P6',
		@type = 'S',
		@enunciado = '¿El/la profesor(a) mantiene una relación de respeto hacia su persona?',
		@justificacion = 'Si no lo respeta, explique por qué';

	EXEC [dbo].[AgregarPreguntaConOpcion]
		@cod = 'CI0128P7',
		@type = 'U',
		@enunciado = '¿Qué tan seguido el/la profesor(a) cumple el horario de lecciones establecido?' 

	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P7', @orden = 0, @texto = 'Nunca';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P7', @orden = 1, @texto = 'Casi nunca';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P7', @orden = 2, @texto = 'A veces';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P7', @orden = 3, @texto = 'Casi siempre';
	EXEC [dbo].[AgregarOpcion] @cod = 'CI0128P7', @orden = 4, @texto = 'Siempre';

END
