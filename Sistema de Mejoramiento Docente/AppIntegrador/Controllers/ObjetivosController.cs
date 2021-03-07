using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using AppIntegrador.Models;
using System.IO;
using AppIntegrador.Utilities;

using AppIntegrador.Controllers.PlanesDeMejoraBI;

namespace AppIntegrador.Controllers
{
    public class ObjetivosController : Controller
    {
        private DataIntegradorEntities db = new DataIntegradorEntities();
        private readonly IPerm permissionManager = new PermissionManager();
        private PlanesDeMejoraUtil util = new PlanesDeMejoraUtil();

        public ObjetivosController() { }
        public ObjetivosController(DataIntegradorEntities db = null, PlanesDeMejoraUtil util = null)
        {
            if(db != null)
            {
                this.db = db;
            }
            if(util != null)
            {
                this.util = util;
            }
        }


        [HttpPost]
        public ActionResult AnadirObjetivos(List<Objetivo> objetivos)
        {
            string result = util.getView(PartialView("_TablaObjetivos", objetivos), this.ControllerContext);
            return Json(new { error = true, message = result });
        }

        [HttpPost]
        public ActionResult ObtenerSecciones(List<String> FormularioSeleccionado)
        {
            List<Formulario_tiene_seccion> parejas = new List<Formulario_tiene_seccion>();
            if(FormularioSeleccionado != null && FormularioSeleccionado.Count > 0)
            {
                foreach (var codigo in FormularioSeleccionado)
                {
                    foreach (var pareja in db.Formulario_tiene_seccion.Where(x => x.FCodigo.Equals(codigo) ) )
                    {
                        parejas.Add(pareja);
                    }
                }
            }
            string result = util.getView(PartialView("_AnadirSecciones", parejas), this.ControllerContext);
            return Json(new { error = true, message = result });
        }

        //se usa
        [HttpGet]
        public PartialViewResult listaDeObjetivos(int id)
        {
            ViewBag.IdPlan = id;
            IEnumerable<AppIntegrador.Models.Objetivo> objetivos = db.Objetivo.Where(o => o.codPlan == id);
            //return PartialView("_listarObjetivos", objetivos);
            return PartialView("_TablaObjetivosLista", objetivos);
        }

        public string TablaSeccionesAsociadas(int id, string objt)
        {

            IEnumerable<string> CodigosSecciones = db.ObtenerSeccionesAsociadasAObjetivo(id, objt);
            List<AppIntegrador.Models.Seccion> Secciones = new List<Seccion>();

            foreach (var cod in CodigosSecciones)
            {
                Secciones.Add(db.Seccion.Find(cod));
            }

            return util.getView(PartialView("_ListaSecciones", Secciones), this.ControllerContext);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
