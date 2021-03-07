using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models.Metadata.RelacionesPlanesFormularios
{
    public class PlanFormulario
    {
        /*Llave del plan de mejora */
        [Display(Name = "CódigoPlan")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public int codPlan { get; set; }

        /*Llave del formulario*/
        [Display(Name = "CódigoFormulario")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public string codFormulario { get; set; }
    }
}