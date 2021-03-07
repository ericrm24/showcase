using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AppIntegrador.Models.Metadata
{
    public class AccionDeMejoraMetadata
    {
        [Display(Name = "Código del Plan")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public int codPlan { get; set; }

        [Display(Name = "Nombre del objetivo")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public string nombreObj { get; set; }
        [MaxLength(250, ErrorMessage = "Este campo acepta un máximo de 250 caracteres")]

        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Display(Name = "Inicio")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> fechaInicio { get; set; }

        [Display(Name = "Fin")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> fechaFin { get; set; }
        public int codPlantilla { get; set; }

        public virtual Objetivo Objetivo { get; set; }
    }
}