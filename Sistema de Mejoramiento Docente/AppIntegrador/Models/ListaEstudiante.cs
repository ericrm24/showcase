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
using System.ComponentModel.DataAnnotations;

namespace AppIntegrador.Models
{
    public class ListaEstudiante
    {
        public string CorreoPersona { get; set; }
        public string IdPersona { get; set; }
        public string TipoIdPersona { get; set; }
        public string NombrePersona { get; set; }
        public string ApellidoPersona { get; set; }
        public string Borrado { get; set; }
        public string CorreoEstudiante { get; set; }
        public string CorreoEstudianteEmpadronado { get; set; }
        public string CodigoCarreraEmpadronado { get; set; }
        public string CodigoEnfasisEmpadronado { get; set; }
    }
}