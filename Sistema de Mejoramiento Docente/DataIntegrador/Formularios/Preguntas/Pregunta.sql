CREATE TABLE Pregunta
(
	Codigo VARCHAR(8) NOT NULL PRIMARY KEY,
	Enunciado NVARCHAR(250) NOT NULL,
	
	Tipo CHAR NOT NULL,
	-- 'U' para preguntas de selección única y 'M' para preguntas de selección múltiple.
	-- 'L' para respuesta libre, 'E' para escalar y 'S' para Si/No/NR.
	CONSTRAINT TipoSeleccion CHECK(Tipo = 'U' OR Tipo = 'M' OR Tipo = 'L' OR Tipo = 'E' OR Tipo = 'S'),
)
