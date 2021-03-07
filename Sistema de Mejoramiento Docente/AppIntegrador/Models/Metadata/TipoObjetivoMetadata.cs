using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models.Metadata
{
    public class TipoObjetivoMetadata
    {
        [Display(Name = "Tipo de Objetivo")]
        public string nombre { get; set; }
    }
}