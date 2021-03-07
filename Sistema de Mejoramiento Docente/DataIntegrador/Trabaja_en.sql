CREATE TABLE [dbo].[Trabaja_en]
(
	[CorreoFuncionario] VARCHAR(50) NOT NULL,
	[CodUnidadAcademica] VARCHAR(10) NOT NULL,
	PRIMARY KEY (CorreoFuncionario, CodUnidadAcademica),
	FOREIGN KEY (CorreoFuncionario) REFERENCES Funcionario (Correo) ON UPDATE CASCADE ON DELETE CASCADE,
	FOREIGN KEY (CodUnidadAcademica) REFERENCES UnidadAcademica (Codigo) ON UPDATE CASCADE ON DELETE CASCADE
)
