using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppIntegrador.Models;
namespace AppIntegrador.Models
{
    public class ResultadosFormulario
    {
        public String CodigoFormulario { get; set; }
        public String SiglaCurso { get; set; }
        public String NumeroGrupo { get; set; }
        public String Semestre { get; set; }
        public String Ano { get; set; }
        public String FechaInicio { get; set; }
        public String FechaFin { get; set; }
        public String Preguntas { get; set; }
        public IEnumerable<SelectListItem> Seccioncitas { get; set; }
    }

    public class Preguntas
    {
        public String codigoSeccion { get; set; }
        public String codigoPregunta { get; set; }
        public String textoPregunta { get; set; }
        public String tipoPregunta { get; set; }
    }

}