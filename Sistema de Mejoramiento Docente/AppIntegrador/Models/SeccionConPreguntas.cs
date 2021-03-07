using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models
{
    public partial class SeccionConPreguntas
    {
        public string CodigoSeccion { get; set; }
        public string Nombre { get; set; }
        public List<PreguntaConNumeroSeccion> Preguntas { get; set; }
        public int Orden { get; set; }
        public IEnumerable<Pregunta> PreguntasNormales { get; set; }
        public bool Edicion { get; set; }
    }
}