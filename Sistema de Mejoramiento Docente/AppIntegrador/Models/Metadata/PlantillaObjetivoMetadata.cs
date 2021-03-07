using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models.Metadata
{
    public class PlantillaObjetivoMetadata
    {
        [Display(Name = "Código de la plantilla")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public int codigo { get; set; }
        [Display(Name = "Nombre del objetivo")]
        public string nombre { get; set; }
        [Display(Name = "Descripción del objetivo")]
        public string descripcion { get; set; }
        [Display(Name = "Tipo de objetivo")]
        public string nombTipoObj { get; set; }
    }
}