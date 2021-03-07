using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppIntegrador.Controllers.PlanesDeMejoraBI;
using AppIntegrador.Models;
using AppIntegrador.Utilities;

namespace AppIntegrador.Controllers
{
    public class AccionDeMejoraController : Controller
    {
        private DataIntegradorEntities db = new DataIntegradorEntities();
        private readonly IPerm permissionManager = new PermissionManager();
        private PlanesDeMejoraUtil util = new PlanesDeMejoraUtil();

        public AccionDeMejoraController() { }
        public AccionDeMejoraController(DataIntegradorEntities db = null, PlanesDeMejoraUtil util = null)
        {
            if (db != null)
            {
                this.db = db;
            }
            if (util != null)
            {
                this.util = util;
            }
        }


        [HttpPost]
        public ActionResult AnadirAccionesDeMejora(List<AccionDeMejora> accionesDeMejora)
        {
            List<AccionDeMejora> misAccionesDeMejora = accionesDeMejora;
            if (misAccionesDeMejora == null)
            {
                misAccionesDeMejora = new List<AccionDeMejora>();
            }
            return Json(new { error = true, message = util.getView(PartialView("_TablaAccionMejora", misAccionesDeMejora), this.ControllerContext) });
        }

        [HttpPost]
        public ActionResult ObtenerPreguntas(List<String> SeccionSeleccionado)
        {
            List<Seccion_tiene_pregunta> parejas = new List<Seccion_tiene_pregunta>();
            if (SeccionSeleccionado != null && SeccionSeleccionado.Count > 0)
            {
                foreach (var codigo in SeccionSeleccionado)
                {
                    foreach (var pareja in db.Seccion_tiene_pregunta.Where(x => x.SCodigo.Equals(codigo)))
                    {
                        parejas.Add(pareja);
                    }
                }
            }            
            string result = util.getView(PartialView("_AnadirPreguntas", parejas), this.ControllerContext);
            return Json(new { error = true, message = result });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AccionesDeObjetivo(int plan, string nombObj, bool edit = true)
        {
            if (!permissionManager.IsAuthorized(Permission.VER_ACCIONES_MEJORA))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página";
                return RedirectToAction("../Home/Index");
            }
            ViewBag.idPlan = plan;
            ViewBag.nombreObj = nombObj;

            IEnumerable<AppIntegrador.Models.AccionDeMejora> acciones = db.AccionDeMejora.Where(o => o.codPlan == plan && o.nombreObj == nombObj);
            if (edit == false)
            {
                return PartialView("_listarAcciones", acciones);
            }
            return PartialView("_accionesDeUnObjetivo", acciones);
        }

        public string TablaPreguntasAsociadas(int id, string objt, string des)
        {

            IEnumerable<string> CodigosPreguntas = db.ObtenerPreguntasDeAccionDeMejora(id, objt, des);
            List<AppIntegrador.Models.Pregunta> preguntas = new List<Pregunta>();

            foreach (var cod in CodigosPreguntas)
            {
                preguntas.Add(db.Pregunta.Find(cod));
            }

            return util.getView(PartialView("_ListaPreguntas", preguntas), this.ControllerContext);
        }
    }
}
