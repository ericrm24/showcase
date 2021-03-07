using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AppIntegrador;
using AppIntegrador.Controllers;
using AppIntegrador.Models;
using Moq;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.IO;
using System.Web.SessionState;
using System.Reflection;
using AppIntegrador.Utilities;
using System.Globalization;

namespace AppIntegrador.Controllers
{
    public class AsignacionFormulariosController : Controller
    {
        // Referencia a la base de datos
        private DataIntegradorEntities db;
        // Referencia al Dashboard controller
        private DashboardController dashboard = new DashboardController();
        private readonly IPerm permissionManager;

        // Controlador por defecto
        public AsignacionFormulariosController()
        {
            db = new DataIntegradorEntities();
            permissionManager = new PermissionManager();
        }

        // Metodo principal de la vista: Index
        [HttpGet]
        public ActionResult Index(string id)
        {
            if (id == null)
            {
                return RedirectToAction("../Formularios/Index");
            }

            if (!permissionManager.IsAuthorized(Permission.CREAR_FORMULARIO))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            if (HttpContext == null)
            {
                return Redirect("~/");
            }
            // Asocia el id del formulario correspondiente
            Formulario formularioDB = db.Formulario.Find(id);
            if (formularioDB == null)
            {
                return View();
            }
            // Almacena el codigo y el nombre del formulario
            ViewBag.Nombre = formularioDB.Nombre;
            ViewBag.Codigo = formularioDB.Codigo;
            return View();
        }


        /**
         * RIP AF1,AF2,AF3,AF4
         * Asignación de formularios: recibe los datos desde la vista 
         * necesarios para la asignar un formulario a uno o más grupos
         */
        [HttpPost]
        public JsonResult Asignar(string codigoFormulario, string codigoUASeleccionada, string codigoCarreraEnfasisSeleccionada, string grupoSeleccionado, string correoProfesorSeleccionado, string fechaInicioSeleccionado, string fechaFinSeleccionado, bool extenderPeriodo, bool enviarCorreos)
        {
            if (!permissionManager.IsAuthorized(Permission.CREAR_FORMULARIO))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return Json(new { error = false, tipoError = 5 });
            }
            // Parámetros que se enviaran al procedimiento almacenado
            string codigoCarrera = null;
            string codigoEnfasis = null;
            string siglaCursoGrupo = null;
            Nullable<byte> numeroGrupo = null;
            Nullable<byte> semestreGrupo = null;
            Nullable<int> anno = null;
            Nullable<DateTime> fechaInicio = null;
            Nullable<DateTime> fechaFin = null;

            // Sino se seleccionan datos, existe un error
            if (codigoUASeleccionada == "null" && codigoCarreraEnfasisSeleccionada == "null" && grupoSeleccionado == "null" && correoProfesorSeleccionado == "null" || (String.IsNullOrEmpty(fechaInicioSeleccionado) && String.IsNullOrEmpty(fechaFinSeleccionado) ))
            {
                return Json(new { error = false, tipoError = 1 });
            }

            if (fechaInicioSeleccionado.Length > 0 && fechaFinSeleccionado.Length > 0)
            {
                fechaInicio = DateTime.ParseExact(fechaInicioSeleccionado, "yyyy-mm-dd", CultureInfo.InvariantCulture);
                fechaFin = DateTime.ParseExact(fechaFinSeleccionado, "yyyy-mm-dd", CultureInfo.InvariantCulture);
            }
            else
            {
                return Json(new { error = false, tipoError = 2 });
            }

            if (DateTime.Compare((DateTime)fechaInicio, (DateTime)fechaFin) > 0)
            {
                return Json(new { error = false, tipoError = 3 });
            }

            // Parsea los strings
            DividirCarreraEnfasis(ref codigoCarreraEnfasisSeleccionada, ref codigoCarrera, ref codigoEnfasis);
            DividirGrupo(ref grupoSeleccionado, ref siglaCursoGrupo, ref numeroGrupo, ref semestreGrupo, ref anno);
            // Conviertes las cadenas "null" en nul
            convertNullStringToNull(ref correoProfesorSeleccionado);
            convertNullStringToNull(ref codigoUASeleccionada);

            // Hace el llamado al procedimiento almacenado que devuelve una tabla con todos los grupos necesarios
            var grupos = db.ObtenerGruposAsociados(codigoUASeleccionada, codigoCarrera, codigoEnfasis, siglaCursoGrupo, numeroGrupo, semestreGrupo, anno, correoProfesorSeleccionado);
            List<ObtenerGruposAsociados_Result> gruposAsociadosLista = null;
            // Se convierte la tabla a lista
            try
            {
                gruposAsociadosLista = grupos.ToList();
                // Si no existen grupos asociados, pues no se pudo asignar
                if (gruposAsociadosLista.Count <= 0)
                {
                    return Json(new { error = false, tipoError = 4 });
                }
            }
            catch
            {
                return Json(new { error = false, tipoError = 4 });
            }

            List<FechasSolapadasInfo> fechasJson = new List<FechasSolapadasInfo>();
            bool haySolapamiento = false;

            foreach(var grupo in grupos)
            {
                FechasSolapadasInfo fechas = VerificarFechasSolapadas(codigoFormulario,
                    grupo.Anno,
                    grupo.Semestre,
                    grupo.NumGrupo,
                    (DateTime)fechaInicio,
                    (DateTime)fechaFin);

                // Si no se solapa, el metodo devuelve null
                if(fechas != null)
                {
                    fechas.PeriodoOriginal.FCodigo = codigoFormulario;
                    fechas.PeriodoOriginal.CSigla = grupo.SiglaCurso;
                    fechas.PeriodoOriginal.GAnno = grupo.Anno;
                    fechas.PeriodoOriginal.GSemestre = grupo.Semestre;
                    fechas.PeriodoOriginal.GNumero = grupo.NumGrupo;

                    // Si las fechas son diferentes, es porque alguna se solapa.
                    if(fechas.FechaInicioNueva != null || fechas.FechaFinNueva != null)
                    {
                        fechasJson.Add(fechas);
                        haySolapamiento = true;
                    }
                }
            }

            string originalInicio = FormularioAsignado.FormatearFecha((DateTime)fechaInicio);
            string originalFin = FormularioAsignado.FormatearFecha((DateTime)fechaFin);

            if (haySolapamiento && !extenderPeriodo)
            {
                originalInicio = FormularioAsignado.FormatearFecha(fechasJson.FirstOrDefault().PeriodoOriginal.FechaInicio);
                originalFin = FormularioAsignado.FormatearFecha(fechasJson.FirstOrDefault().PeriodoOriginal.FechaFin);

                return Json(new { error = false, tipoError = 6, inicio = originalInicio, fin = originalFin });
            }

            // Itera por los grupos obtenidos, asignandoles el formulario
            for (int index = 0; index < gruposAsociadosLista.Count; ++index)
            {
                var grupoActual = gruposAsociadosLista[index];
                // Procedimiento que almacena en las relaciones Activa_Por, Activa_Por_Periodo
                db.AsignarFormulario(codigoFormulario, grupoActual.SiglaCurso, grupoActual.NumGrupo, grupoActual.Anno, grupoActual.Semestre, fechaInicio, fechaFin);
            }

            if(haySolapamiento)
            {
                FechasSolapadasInfo fecha = fechasJson.FirstOrDefault();
                if(fecha.FechaInicioNueva != null)
                {
                    originalInicio = FormularioAsignado.FormatearFecha((DateTime)fechaInicio);
                }
                else
                {
                    originalInicio = FormularioAsignado.FormatearFecha(fecha.PeriodoOriginal.FechaInicio);
                }

                if(fecha.FechaFinNueva != null)
                {
                    originalFin = FormularioAsignado.FormatearFecha((DateTime)fechaFin);
                }
                else
                {
                    originalFin = FormularioAsignado.FormatearFecha(fecha.PeriodoOriginal.FechaFin);
                }
            }

            if (enviarCorreos)
            {
                var estudiantes = db.ObtenerEstudiantesAsociados(codigoUASeleccionada, codigoCarrera, codigoEnfasis, siglaCursoGrupo, numeroGrupo, semestreGrupo, anno, correoProfesorSeleccionado);
                List<ObtenerEstudiantesAsociados_Result> estudiantesAsociadosLista = null;
                // Se convierte la tabla a lista
                try
                {
                    estudiantesAsociadosLista = estudiantes.ToList();
                    // Si no existen grupos asociados, pues no se pudo asignar
                    if (estudiantesAsociadosLista.Count <= 0)
                    {
                        return Json(new { error = false, tipoError = 4 });
                    }
                }
                catch
                {
                    return Json(new { error = false, tipoError = 4 });
                }

                EnviarCorreoSobreAsignaciónCuestionario(estudiantesAsociadosLista, db.Formulario.Find(codigoFormulario) );
            }

            return Json(new { error = true, inicio = originalInicio, fin = originalFin });
        }

        private bool convertNullStringToNull(ref string convertedString)
        {
            if (convertedString == "null")
            {
                convertedString = null;
            }
            return false;
        }
        private bool DividirCarreraEnfasis(ref string codigoCarreraEnfasisSeleccionada, ref string codigoCarrera, ref string codigoEnfasis)
        {

            if (codigoCarreraEnfasisSeleccionada != "null")
            {
                string[] codigosSeparados = codigoCarreraEnfasisSeleccionada.Split('/');
                codigoCarrera = codigosSeparados[0];
                codigoEnfasis = codigosSeparados[1];
                return true;
            }
            return false;
        }

        private bool DividirGrupo(ref string grupoSeleccionado, ref string siglaCursoGrupo, ref Nullable<byte> numeroGrupo, ref Nullable<byte> semestreGrupo, ref Nullable<int> anno)
        {
            if (grupoSeleccionado != "null")
            {
                string[] grupoLlaves = grupoSeleccionado.Split('/');
                siglaCursoGrupo = grupoLlaves[0];
                numeroGrupo = byte.Parse(grupoLlaves[1]);
                semestreGrupo = byte.Parse(grupoLlaves[2]);
                anno = int.Parse(grupoLlaves[3]);
                return true;
            }
            return false;
        }


        // Modificado por: Jostin Álvarez
        // Historia a la que pertenece: RIP-AFC "Yo como administrativo quiero enviar un correo a los estudiantes para que llenen formularios cuando se los asigno"
        // Envía un correo cada estudiando que está asignado a un cuestionario pidiéndole que lo llene.
        private void EnviarCorreoSobreAsignaciónCuestionario(List<ObtenerEstudiantesAsociados_Result> estudiantes, Formulario formulario)
        {
            List<string> involucrados = new List<string>();

            foreach (ObtenerEstudiantesAsociados_Result estudiante in estudiantes)
            {
                involucrados.Add(estudiante.Correo);
            }
            Utilities.EmailNotification emailNotification = new Utilities.EmailNotification();

            string asunto = "Evaluación del formulario " + formulario.Nombre;

            string texto = "Estimado/a estudiante\n";
            texto += "La Escuela de Ciencias de la Computación e Informática le solicita ";
            texto += "que conteste el formulario " + formulario.Codigo + ": " + formulario.Nombre;
            texto += "<br>Favor no responder directamente a este correo";

            string textoAlt = "<body><p>" + texto + "</p></body>";

            // Se envía el correo formado a todos los estudiantes involucrados
            _ = emailNotification.SendNotification(involucrados, asunto, texto, textoAlt);
        }

        public FechasSolapadasInfo VerificarFechasSolapadas(string FCodigo,
                                                    int GAnno,            
                                                    byte GSemestre,
                                                    byte GNumero,
                                                    DateTime fechaInicio,
                                                    DateTime fechaFin)
        {
            // Verificar si se solapa la fecha de inicio del form
            var inicioSolapada = from p in db.Periodo_activa_por
                                 where p.FCodigo == FCodigo &&
                                 p.GAnno == GAnno &&
                                 p.GSemestre == GSemestre &&
                                 p.GNumero == GNumero &&
                                 (fechaInicio <= p.FechaInicio && p.FechaInicio <= fechaFin)
                                 select p.FechaInicio;                         

            // Verificar si se solapa la fecha de fin del form
            var finSolapada = from p in db.Periodo_activa_por
                              where p.FCodigo == FCodigo &&
                              p.GAnno == GAnno &&
                              p.GSemestre == GSemestre &&
                              p.GNumero == GNumero &&
                              (fechaInicio <= p.FechaFin && p.FechaFin <= fechaFin)
                              select p.FechaFin;


            FechasSolapadasInfo fechas = new FechasSolapadasInfo();
            Periodo_activa_por original = new Periodo_activa_por();
            
            if(!inicioSolapada.Any() && !finSolapada.Any())
            { 
                return null;
            }

            // Si la fecha se solapa
            if(inicioSolapada.Any())
            {
                // Guardo la original
                original.FechaInicio = inicioSolapada.FirstOrDefault();
                // Guardo la nueva
                fechas.FechaInicioNueva = fechaInicio;

                if(!finSolapada.Any())
                {
                    original.FechaFin = (from p in db.Periodo_activa_por
                                        where p.FCodigo == FCodigo &&
                                        p.GAnno == GAnno &&
                                        p.GSemestre == GSemestre &&
                                        p.GNumero == GNumero &&
                                        p.FechaInicio == original.FechaInicio
                                        select p.FechaFin).FirstOrDefault();
                }
            }

            // Si la fecha se solapa
            if(finSolapada.Any())
            {
                // Guardo la original
                original.FechaFin = finSolapada.FirstOrDefault();
                // Guardo la nueva
                fechas.FechaFinNueva = fechaFin;

                if(!inicioSolapada.Any())
                {
                    original.FechaInicio = (from p in db.Periodo_activa_por
                                         where p.FCodigo == FCodigo &&
                                         p.GAnno == GAnno &&
                                         p.GSemestre == GSemestre &&
                                         p.GNumero == GNumero &&
                                         p.FechaFin == original.FechaFin
                                         select p.FechaInicio).FirstOrDefault();
                }
            }

            fechas.PeriodoOriginal = original;

            return fechas;
        }
    }
}
