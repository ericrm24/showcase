using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models.Metadata
{
    public class ObjetivoMetadata
    {
        [Display(Name = "Código Plan")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public int codPlan { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public string nombre { get; set; }

        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Inicio")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> fechaInicio { get; set; }

        [Display(Name = "Fin")]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> fechaFin { get; set; }

        [Display(Name = "Tipo")]
        public string nombTipoObj { get; set; }

        [Display(Name = "Código plantilla")]
        public Nullable<int> codPlantilla { get; set; }
    }
}