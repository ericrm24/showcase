using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppIntegrador.Models;

namespace AppIntegrador.Models
{
    public class FechasSolapadasInfo
    {
        public Periodo_activa_por PeriodoOriginal { get; set; }

        public DateTime? FechaInicioNueva { get; set; }

        public DateTime? FechaFinNueva { get; set; }

        public FechasSolapadasInfo()
        {
            this.PeriodoOriginal = null;
            this.FechaInicioNueva = null;
            this.FechaFinNueva = null;
        }

        public FechasSolapadasInfo(Periodo_activa_por periodo, DateTime inicio, DateTime fin)
        {
            this.PeriodoOriginal = periodo;
            this.FechaInicioNueva = inicio;
            this.FechaFinNueva = fin;
        }

    }
}