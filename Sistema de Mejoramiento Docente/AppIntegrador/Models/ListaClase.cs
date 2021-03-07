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
    public class ListaClase
    {
        public string CodigoUnidad { get; set; }
        public string NombreFacultad { get; set; }
        public string CodigoUnidadCarrera { get; set; }
        public string CodigoCarreraUnidad { get; set; }
        public string CodigoCarrera { get; set; }
        public string NombreCarrera { get; set; }
        public string CodigoCarreraEnfasis { get; set; }
        public string CodigoEnfasis { get; set; }
        public string NombreEnfasis { get; set; }
    }
}