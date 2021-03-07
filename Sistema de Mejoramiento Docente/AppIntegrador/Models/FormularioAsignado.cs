using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models
{
    public class FormularioAsignado
    {
        public string Correo { get; }
        public Periodo_activa_por Periodo { get; }
        public string Nombre { get; }
        public string NombreCurso { get;}
        public int Estado { get; }

        public string FechaInicioFormateada { get;  }
        public string FechaFinFormateada { get; }

        public FormularioAsignado()
        {
            this.Periodo = null;
            this.Nombre = "Formulario inválido";
            this.Correo = "error";
        }

        public FormularioAsignado(Periodo_activa_por periodo, string Correo)
        {
            // Guardar correo desde el httpcontext
            this.Correo = Correo;

            // Guardar objeto de periodo
            this.Periodo = periodo;

            // Basado en el objeto, guardar nombre del formulario
            using (DataIntegradorEntities db = new DataIntegradorEntities())
            {
                this.Nombre = db.Formulario.Find(this.Periodo.FCodigo).Nombre;

                this.NombreCurso = db.Curso.Find(this.Periodo.CSigla).Nombre;

                this.Estado = ObtenerEstadoFormulario();

                db.Dispose();
            }

            this.FechaInicioFormateada = FormatearFecha(this.Periodo.FechaInicio);
            this.FechaFinFormateada = FormatearFecha(this.Periodo.FechaFin);
        }

        private int ObtenerEstadoFormulario()
        {
            /*
             * Estado 0 = En progreso
             * Estado 1 = finalizado
             * Estado 2 = No disponible
             * Estado 3 = Disponible
             */

            DataIntegradorEntities db = new DataIntegradorEntities();
            int estado = 0;
            var fecha = DateTime.Now;

            if (this.Periodo.FechaFin < fecha)
            {
                estado = 1;
            }
            else if (this.Periodo.FechaInicio > fecha)
            {
                estado = 2;
            }
            else
            {
                var estadoDB = from r in db.Respuestas_a_formulario
                               where r.FCodigo == Periodo.FCodigo && r.Correo == this.Correo
                               && r.CSigla == Periodo.CSigla && r.GNumero == Periodo.GNumero
                               && r.GAnno == Periodo.GAnno && r.GSemestre == Periodo.GSemestre
                               && (this.Periodo.FechaInicio <= r.Fecha && r.Fecha <= this.Periodo.FechaFin)
                               select r.Finalizado;

                if(estadoDB.Any())
                {
                    // Si el formulario está finalizado
                    if(estadoDB.FirstOrDefault() == true)
                    {
                        estado = 1;
                    }
                    else // Si no, está en progreso
                    {
                        estado = 0;
                    }
                }
                else // Si no existe respuesta, pero está en periodo de llenado, se muestra disponible
                {
                    estado = 3;
                }
            }

            db.Dispose();

            return estado;
        }

        public static string FormatearFecha(DateTime fecha)
        {
            return fecha.Day.ToString(CultureInfo.CreateSpecificCulture("es")) + " de " +
                fecha.ToString("MMMMMMMMMMMMM", CultureInfo.CreateSpecificCulture("es")) + " de " +
                fecha.Year.ToString(CultureInfo.CreateSpecificCulture("es")) + ", " +
                fecha.ToString("hh", CultureInfo.CreateSpecificCulture("es")) + ":" +
                fecha.ToString("mm", CultureInfo.CreateSpecificCulture("es")) + " " +
                fecha.ToString("tt", CultureInfo.InvariantCulture);
        }
    }
}