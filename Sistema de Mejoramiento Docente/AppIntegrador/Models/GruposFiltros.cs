using System;

namespace AppIntegrador.Models
{
    public partial class GruposFiltros
    {
        public string SiglaCurso { get; set; }
        public string NombreCurso { get; set; }
        public Nullable<byte> NumGrupo { get; set; }
        public Nullable<byte> Semestre { get; set; }
        public Nullable<int> Anno { get; set; }
    }
}