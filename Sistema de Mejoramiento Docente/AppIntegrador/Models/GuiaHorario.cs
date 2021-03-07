using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LINQtoCSV;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using AppIntegrador.Models;

namespace AppIntegrador.Models
{
    public class GuiaHorario
    {
        public string CodigoCarreraCurso { get; set; }
        public string CodigoEnfasisCurso { get; set; }
        public string SiglaCursoCarrera { get; set; }
        public string SiglaCurso { get; set; }
        public string NombreCurso { get; set; }
        public string SiglaCursoGrupo { get; set; }
        public string NumeroGrupo { get; set; }
        public string Semestre { get; set; }
        public string Anno { get; set; }
        public string CorreoMatricula { get; set; }
        public string SiglaCursoMatricula { get; set; }
        public string NumeroGrupoMatricula { get; set; }
        public string SemestreMatricula { get; set; }
        public string AnnoMatricula { get; set; }
    }
}