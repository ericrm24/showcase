using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models.Metadata
{
    public class PermisoMetadata
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        [Display(Name = "Asignado a Perfil")]
        public bool ActiveInProfileEmph { get; set; }
    }
}