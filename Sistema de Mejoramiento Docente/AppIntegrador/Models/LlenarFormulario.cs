using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models
{
    public class LlenarFormulario
    {
        public Formulario Formulario { get; set; }
        public List<SeccionConPreguntas> Secciones { get; set; }
        public Grupo Grupo { get; set; }
    }
}