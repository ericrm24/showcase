using System;

namespace AppIntegrador.Models
{
    public partial class FormulariosFiltros
    {
        public string FCodigo { get; set; }
        public string FNombre { get; set; }
        public string CSigla { get; set; }
        public Nullable<byte> GNumero { get; set; }
        public Nullable<byte> GSemestre { get; set; }
        public Nullable<int> GAnno { get; set; }
        public Nullable<System.DateTime> FechaInicio { get; set; }
        public Nullable<System.DateTime> FechaFin { get; set; }
    }
}