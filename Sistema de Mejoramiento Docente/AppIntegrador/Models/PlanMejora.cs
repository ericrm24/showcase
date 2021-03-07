using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AppIntegrador.Models
{
    public class PlanMejora
    {
        public int codigo { get; set; }

        [Required(ErrorMessage = "Este campo es requerido!!")]
        [Display(Name = "Nombre")]
        [DataType(DataType.Text)]
        [MaxLength(30, ErrorMessage = "La longitud máxima de este campo es de 30 caracteres")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "Este campo es requerido!!")]
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fechaInicio { get; set; }

        [Required(ErrorMessage = "Este campo es requerido!!")]
        [Display(Name = "Fecha Finalización")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fechaFin { get; set; }
    }
}