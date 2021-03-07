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
    public class ListaFuncionario
    {
        public string CorreoPersona { get; set; }
        public string IdPersona { get; set; }
        public string TipoIdPersona { get; set; }
        public string NombrePersona { get; set; }
        public string ApellidoPersona { get; set; }
        public string Borrado { get; set; }
        public string CorreoFuncionario { get; set; }
        public string CorreoProfesor { get; set; }
        public string CorreoFuncionarioTrabaja { get; set; }
        public string CodigoUnidadTrabaja { get; set; }
    }
}