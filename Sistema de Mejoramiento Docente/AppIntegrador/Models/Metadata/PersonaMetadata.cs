using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models.Metadata
{
    public class PersonaMetadata
    {
        [Display(Name = "Correo Electrónico")]
        [StringLength(50, ErrorMessage = "El correo no puede exceder los 50 caracteres.")]
        [EmailAddress(ErrorMessage = "Dirección de correo inválida.")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string Correo { get; set; }
        [Display(Name = "Correo Alternativo")]
        [EmailAddress(ErrorMessage = "Dirección de correo inválida.")]
        [StringLength(50, ErrorMessage = "El correo no puede exceder los 50 caracteres.")]
        public string CorreoAlt { get; set; }
        [StringLength(30, ErrorMessage = "La identificación no puede exceder los 30 caracteres.")]
        [Display(Name = "Identificación")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string Identificacion { get; set; }
        [Display(Name = "Primer Nombre")]
        [StringLength(15, ErrorMessage = "Este campo no puede exceder los 15 caracteres.")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string Nombre1 { get; set; }
        [Display(Name = "Segundo Nombre")]
        [StringLength(15, ErrorMessage = "Este campo no puede exceder los 15 caracteres.")]
        public string Nombre2 { get; set; }
        [Display(Name = "Primer Apellido")]
        [StringLength(15, ErrorMessage = "Este campo no puede exceder los 15 caracteres.")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string Apellido1 { get; set; }
        [Display(Name = "Segundo Apellido")]
        [StringLength(15, ErrorMessage = "Este campo no puede exceder los 15 caracteres.")]
        public string Apellido2 { get; set; }
        [Display(Name = "Tipo de Identificación")]
        public string TipoIdentificacion { get; set; }
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; }
        [Display(Name = "Perfil asignado")]
        public bool HasProfileInEmph { get; set; }

        private static void MyContext()
        {
            var type = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
            if (type == null)
                throw new Exception("Do not remove, ensures static reference to System.Data.Entity.SqlServer");
        }
    }
}