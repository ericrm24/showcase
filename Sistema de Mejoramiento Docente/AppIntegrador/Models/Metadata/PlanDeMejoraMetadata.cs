using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AppIntegrador.Models.Metadata
{
    [MetadataType(typeof(PlanDeMejoraMetadata))]
    public partial class PlanDeMejora
    {
    }

    public class PlanDeMejoraMetadata
    {
        [Key]
        [Display(Name = "Código")]
        [Required(ErrorMessage = "Este campo es necesario")]
        [Range(0, int.MaxValue, ErrorMessage = "Este valor debe ser un número entero no negativo")]
        public int codigo { get; set; }

        [Display(Name = "Nombre")]
        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessage = "La longitud máxima de este campo es de 30 caracteres")]
        [Required]
        public string nombre { get; set; }

        [Display(Name = "Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        public Nullable<System.DateTime> fechaInicio { get; set; }

        [Display(Name = "Fin")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]        
        public Nullable<System.DateTime> fechaFin { get; set; }

        [Display(Name = "Profesores Asignados")]
        public virtual ICollection<Profesor> Profesor { get; set; }


    }
}