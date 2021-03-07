using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models
{
    public partial class PreguntaConNumeroSeccion
    {
        public int OrdenSeccion { get; set; }
        public string CodigoSeccion { get; set; }
        public int OrdenPregunta { get; set; }
        public Pregunta Pregunta { get; set; }
        public List<int> Opciones { get; set; }
        public string RespuestaLibreOJustificacion { get; set; }
        public bool Edit { get; set; }
    }
}