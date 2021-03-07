using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models
{
    public class MisFormulariosModel
    {
        public List<FormularioAsignado> FormulariosSemestre { get; }
        public List<FormularioAsignado> FormulariosPasados { get; }

        public MisFormulariosModel()
        {
            FormulariosSemestre = new List<FormularioAsignado>();
            FormulariosPasados = new List<FormularioAsignado>();
        }

        public void InsertarPasado(FormularioAsignado formulario)
        {
            if(formulario == null)
            {
                return;
            }

            foreach(var actual in FormulariosSemestre)
            {
                if(SonIguales(actual, formulario))
                {
                    return;
                }
            }

            this.FormulariosPasados.Add(formulario);
        }

        private bool SonIguales(FormularioAsignado primero, FormularioAsignado segundo)
        {
            return primero.Periodo.FechaInicio == segundo.Periodo.FechaInicio &&
                primero.Periodo.FechaFin == segundo.Periodo.FechaFin &&
                primero.Periodo.CSigla == segundo.Periodo.CSigla &&
                primero.Periodo.FCodigo == segundo.Periodo.FCodigo &&
                primero.Periodo.GAnno == segundo.Periodo.GAnno &&
                primero.Periodo.GSemestre == segundo.Periodo.GSemestre &&
                primero.Periodo.GNumero == segundo.Periodo.GNumero;
        }
    }
}