CREATE TABLE [dbo].[UnidadAcademica]
(
	[Codigo] VARCHAR(10) NOT NULL PRIMARY KEY, 
    [Nombre] VARCHAR(50) NOT NULL, 
    [Superior] VARCHAR(10) NULL
	FOREIGN KEY (Superior) REFERENCES UnidadAcademica (Codigo) ON UPDATE NO ACTION ON DELETE NO ACTION
)
