using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models.Metadata
{

    /*
        Modificado por: Christian Asch (30/10/19)
        Historia a la que pertenece: MOS-1.11 "agregar, modificar, borrar y listar los accionables de una acción de mejora"
    */
    public class AccionableMetadata
    {
        [Display(Name = "Código del plan")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public int codPlan { get; set; }

        [Display(Name = "Nombre del objetivo")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public string nombreObj { get; set; }

        [Display(Name = "Descripción de la acción de mejora")]
        public string descripAcMej { get; set; }

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
        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Este campo es requerido")]
        public string tipo { get; set; }
        [Display(Name = "Peso")]
        public Nullable<int> peso { get; set; }

        [Display(Name = "Porcentaje de peso")]
        [Range(0, 100, ErrorMessage = "El porcentaje es un número entre 0 y 100")]
        public Nullable<int> pesoPorcentaje { get; set; }
    }
}