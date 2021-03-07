using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppIntegrador.Models;
using AppIntegrador.Utilities;

//MOS-8 Como Usuario administrativo	quiero poder agregar tipos de objetivos para dar opciones a la hora de crear los objetivos
//Tarea 1: "1. Es necesario agregar un scaffold de las operaciones de CRUD de los tipos de objetivos
//Christian Asch
//Commits: 29298cf, c0d43bd, e4023d4

namespace AppIntegrador.Controllers
{
    public class TipoObjetivosController : Controller
    {
        private DataIntegradorEntities db = new DataIntegradorEntities();
        private readonly IPerm permissionManager;

        public TipoObjetivosController()
        {
            permissionManager = new PermissionManager();
        }
        //Para pruebas
        public TipoObjetivosController(DataIntegradorEntities db)
        {
            this.db = db;
            permissionManager = new PermissionManager();
        }


        // GET: TipoObjetivos
        public ActionResult Index()
{
            if (!permissionManager.IsAuthorized(Permission.VER_OBJETIVOS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página";
                return RedirectToAction("../Home/Index");
            }
            return View("Index", db.TipoObjetivo.ToList());
        }

        // GET: TipoObjetivos/Create
        [HttpGet]
        public ActionResult Create()
        {
            if (!permissionManager.IsAuthorized(Permission.CREAR_OBJETIVOS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página";
                return RedirectToAction("../Home/Index");
            }
            return View("Crear");
        }

        // POST: TipoObjetivos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nombre")] TipoObjetivo tipoObjetivo)
        {
            if (!permissionManager.IsAuthorized(Permission.CREAR_OBJETIVOS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página";
                return RedirectToAction("../Home/Index");
            }
            if (ModelState.IsValid)
            {
                db.TipoObjetivo.Add(tipoObjetivo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoObjetivo);
        }
        //Para pruebas
        public ActionResult Create([Bind(Include = "nombre")] TipoObjetivo tipoObjetivo, DataIntegradorEntities db)
        {
            if (!permissionManager.IsAuthorized(Permission.CREAR_OBJETIVOS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página";
                return RedirectToAction("../Home/Index");
            }
            if (ModelState.IsValid)
            {
                db.TipoObjetivo.Add(tipoObjetivo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoObjetivo);
        }



        // GET: TipoObjetivos/Edit/5
        public ActionResult Edit(string id)
        {
            if (!permissionManager.IsAuthorized(Permission.EDITAR_OBJETIVOS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página";
                return RedirectToAction("../Home/Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoObjetivo tipoObjetivo = db.TipoObjetivo.Find(id);
            if (tipoObjetivo == null)
            {
                return HttpNotFound();
            }
            return View("Editar", tipoObjetivo);
        }

        // POST: TipoObjetivos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "nombre")] TipoObjetivo tipoObjetivo)
        {
            if (!permissionManager.IsAuthorized(Permission.EDITAR_OBJETIVOS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página";
                return RedirectToAction("../Home/Index");
            }
            if (ModelState.IsValid)
            {
                db.Entry(tipoObjetivo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoObjetivo);
        }

        // GET: TipoObjetivos/Delete/5
        public ActionResult Delete(string id)
        {
            if (!permissionManager.IsAuthorized(Permission.BORRAR_OBJETIVOS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página";
                return RedirectToAction("../Home/Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoObjetivo tipoObjetivo = db.TipoObjetivo.Find(id);
            if (tipoObjetivo == null)
            {
                return HttpNotFound();
            }
            return View("Eliminar", tipoObjetivo);
        }

        // POST: TipoObjetivos/Delete/5
        //[HttpPost, ActionName("DeleteConfirmed")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, bool confirmed)
        {
            if (!permissionManager.IsAuthorized(Permission.VER_OBJETIVOS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página";
                return RedirectToAction("../Home/Index");
            }
            if (confirmed)
            {
                TipoObjetivo tipoObjetivo = db.TipoObjetivo.Find(id);
                db.TipoObjetivo.Remove(tipoObjetivo);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
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
