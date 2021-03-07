using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models.Metadata
{
    public class EstudianteMetadata
    {
        public string Correo { get; set; }
        [Display(Name = "Carné Estudiantil")]
        public string Carne { get; set; }
    }
}