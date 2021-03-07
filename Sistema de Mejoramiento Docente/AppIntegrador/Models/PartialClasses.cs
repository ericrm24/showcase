using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using AppIntegrador.Models.Metadata;

namespace AppIntegrador.Models
{
    [MetadataType(typeof(PlanDeMejoraMetadata))]
    public partial class PlanDeMejora { }

    [MetadataType(typeof(AccionDeMejoraMetadata))]
    public partial class AccionDeMejora { }

    [MetadataType(typeof(ObjetivoMetadata))]
    public partial class Objetivo { }

    [MetadataType(typeof(TipoObjetivoMetadata))]
    public partial class TipoObjetivo { }

    [MetadataType(typeof(PlantillaObjetivoMetadata))]
    public partial class PlantillaObjetivo { }

    [MetadataType(typeof(AccionableMetadata))]
    public partial class Accionable { }

    [MetadataType(typeof(PersonaMetadata))]
    public partial class Persona
    {
        public string NombreCompleto { get; set; }
        // En la administración de Perfiles indica si el usuario tiene ese perfil en el énfasis
        public bool HasProfileInEmph { get; set; }
    }

    [MetadataType(typeof(EstudianteMetadata))]
    public partial class Estudiante { }

    [MetadataType(typeof(PermisoMetadata))]
    public partial class Permiso
    {
        // En la administración de Perfiles indica si el perfil tiene activo el permiso en el énfasis
        public bool ActiveInProfileEmph { get; set; }
    }
}