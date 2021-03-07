using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppIntegrador.Models;

namespace AppIntegrador.Controllers
{
    public class ActivaFormularioController : Controller
    {
        private DataIntegradorEntities db = new DataIntegradorEntities();

        // GET: ActivaFormulario
        public ActionResult Index()
        {
            var activa_por = db.Activa_por.Include(a => a.Formulario).Include(a => a.Grupo);
            return View(activa_por.ToList());
        }

        // GET: ActivaFormulario/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activa_por activa_por = db.Activa_por.Find(id);
            if (activa_por == null)
            {
                return HttpNotFound();
            }
            return View(activa_por);
        }

        // GET: ActivaFormulario/Create
        public ActionResult Create()
        {
            ViewBag.FCodigo = new SelectList(db.Formulario, "Codigo", "Nombre");
            ViewBag.CSigla = new SelectList(db.Grupo, "SiglaCurso", "SiglaCurso");
            ViewBag.GNumero = new SelectList(db.Grupo, "NumGrupo", "NumGrupo");
            ViewBag.GAnno = new SelectList(db.Grupo, "Anno", "Anno");
            ViewBag.GSemestre = new SelectList(db.Grupo, "Semestre", "Semestre");


            return View();
        }

        // POST: ActivaFormulario/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FCodigo,CSigla,GNumero,GAnno,GSemestre")] Activa_por activa_por)
        {
            if (ModelState.IsValid)
            {
                db.Activa_por.Add(activa_por);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FCodigo = new SelectList(db.Formulario, "Codigo", "Nombre", activa_por.FCodigo);
            ViewBag.CSigla = new SelectList(db.Grupo, "SiglaCurso", "SiglaCurso", activa_por.CSigla);
            return View(activa_por);
        }

        // GET: ActivaFormulario/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activa_por activa_por = db.Activa_por.Find(id);
            if (activa_por == null)
            {
                return HttpNotFound();
            }
            ViewBag.FCodigo = new SelectList(db.Formulario, "Codigo", "Nombre", activa_por.FCodigo);
            ViewBag.CSigla = new SelectList(db.Grupo, "SiglaCurso", "SiglaCurso", activa_por.CSigla);
            return View(activa_por);
        }

        // POST: ActivaFormulario/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FCodigo,CSigla,GNumero,GAnno,GSemestre")] Activa_por activa_por)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activa_por).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FCodigo = new SelectList(db.Formulario, "Codigo", "Nombre", activa_por.FCodigo);
            ViewBag.CSigla = new SelectList(db.Grupo, "SiglaCurso", "SiglaCurso", activa_por.CSigla);
            return View(activa_por);
        }

        // GET: ActivaFormulario/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activa_por activa_por = db.Activa_por.Find(id);
            if (activa_por == null)
            {
                return HttpNotFound();
            }
            return View(activa_por);
        }

        // POST: ActivaFormulario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Activa_por activa_por = db.Activa_por.Find(id);
            db.Activa_por.Remove(activa_por);
            db.SaveChanges();
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
