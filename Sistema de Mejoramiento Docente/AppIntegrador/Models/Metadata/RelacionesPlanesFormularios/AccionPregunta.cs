using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models.Metadata.RelacionesPlanesFormularios
{
    public class AccionPregunta
    {
        /* Llave de la accion de mejora */
        [Display(Name = "Código Plan Acción de Mejora")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public int codPlanAccionDeMejora { get; set; }

        [Display(Name = "Código Acción de Mejora")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public string nombreObjetivo { get; set; }

        [Display(Name = "Código Acción de Mejora")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public string descripcion { get; set; }

        /* Llave de la pregunta */
        [Display(Name = "Código Acción de Mejora")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public string codPregunta { get; set; }
    }
}