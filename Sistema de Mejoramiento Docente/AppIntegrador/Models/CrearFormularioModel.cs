namespace AppIntegrador.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using AppIntegrador.Models;
    using System.Collections.Generic;

    public partial class CrearFormularioModel : DbContext
    {
        public Formulario Formulario { get; set; }
        public CrearSeccionModel crearSeccionModel { get; set; }
        public IEnumerable<Seccion> seccion { get; set; }
        public IEnumerable<Pregunta> preguntas{ get; set; }
        public SeccionConPreguntas seccionConPreguntas{ get; set; }
        

        public List<SeccionConPreguntas> seccionesConPreguntas { get; set; }

        public bool Creado { get; set; }
    }


}