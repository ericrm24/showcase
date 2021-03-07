/*Representa las unidades académicas que se pasan como parámetro a las funciones de los filtros.*/
CREATE TYPE FiltroUnidadesAcademicas
AS TABLE (CodigoUA VARCHAR(10) PRIMARY KEY);

GO

/*Representa las carreras y énfasis que se pasan como parámetro a las funciones de los filtros.*/
CREATE TYPE FiltroCarrerasEnfasis
AS TABLE (CodigoCarrera VARCHAR(10), CodigoEnfasis VARCHAR(10), PRIMARY KEY (CodigoCarrera, CodigoEnfasis));

GO

/*Representa los grupos que se pasan como parámetro a las funciones de los filtros.*/
CREATE TYPE FiltroGrupos
AS TABLE (SiglaCurso VARCHAR(10), NumeroGrupo TINYINT, Semestre TINYINT, Anno INT, PRIMARY KEY (SiglaCurso, NumeroGrupo, Semestre, Anno));

GO

/*Representa profesores que se pasan como parámetro a las funciones de los filtros.*/
CREATE TYPE FiltroProfesores
AS TABLE (CorreoProfesor VARCHAR(50) PRIMARY KEY);