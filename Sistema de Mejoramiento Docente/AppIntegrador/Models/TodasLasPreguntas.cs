using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models
{
    public class TodasLasPreguntas
    {
        // Se guarda una lista con el enunciado de cada pregunta y su propia lista de opciones
        public List<PreguntaConNumeroSeccion> Preguntas { get; set; }

        // Se ocupa este dato para poder separar los radio buttons en la vista
        public string CodigoSeccion { get; set; }
    }
}