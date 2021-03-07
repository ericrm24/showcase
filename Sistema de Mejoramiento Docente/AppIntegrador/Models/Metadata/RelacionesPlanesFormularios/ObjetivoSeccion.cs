using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models.Metadata.RelacionesPlanesFormularios
{
    public class ObjetivoSeccion
    {
        /*Llave del objetivo*/
        [Display(Name = "Código Objetivo")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public int codPlan { get; set; }

        [Display(Name = "Nombre Objetivo")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public string nombreObjetivo { get; set; }

        /*Llave de la sección*/
        [Display(Name = "Código Sección")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public string codSeccion { get; set; }
    }
}