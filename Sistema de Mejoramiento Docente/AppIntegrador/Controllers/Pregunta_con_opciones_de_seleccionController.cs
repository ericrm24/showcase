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
    public class Pregunta_con_opciones_de_seleccionController : Controller
    {
        private DataIntegradorEntities db = new DataIntegradorEntities();

        // GET: Pregunta_con_opciones_de_seleccion
        public ActionResult Index()
        {
            var pregunta_con_opciones_de_seleccion = db.Pregunta_con_opciones_de_seleccion.Include(p => p.Pregunta_con_opciones);
            return View(pregunta_con_opciones_de_seleccion.ToList());
        }

        // GET: Pregunta_con_opciones_de_seleccion/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta_con_opciones_de_seleccion pregunta_con_opciones_de_seleccion = db.Pregunta_con_opciones_de_seleccion.Find(id);
            if (pregunta_con_opciones_de_seleccion == null)
            {
                return HttpNotFound();
            }
            return View(pregunta_con_opciones_de_seleccion);
        }

        // GET: Pregunta_con_opciones_de_seleccion/Create
        public ActionResult Create()
        {
            ViewBag.Codigo = new SelectList(db.Pregunta_con_opciones, "Codigo", "TituloCampoObservacion");
            return View();
        }

        // POST: Pregunta_con_opciones_de_seleccion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Codigo,Tipo")] Pregunta_con_opciones_de_seleccion pregunta_con_opciones_de_seleccion)
        {
            if (ModelState.IsValid)
            {
                db.Pregunta_con_opciones_de_seleccion.Add(pregunta_con_opciones_de_seleccion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Codigo = new SelectList(db.Pregunta_con_opciones, "Codigo", "TituloCampoObservacion", pregunta_con_opciones_de_seleccion.Codigo);
            return View(pregunta_con_opciones_de_seleccion);
        }

        // GET: Pregunta_con_opciones_de_seleccion/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta_con_opciones_de_seleccion pregunta_con_opciones_de_seleccion = db.Pregunta_con_opciones_de_seleccion.Find(id);
            if (pregunta_con_opciones_de_seleccion == null)
            {
                return HttpNotFound();
            }
            ViewBag.Codigo = new SelectList(db.Pregunta_con_opciones, "Codigo", "TituloCampoObservacion", pregunta_con_opciones_de_seleccion.Codigo);
            return View(pregunta_con_opciones_de_seleccion);
        }

        // POST: Pregunta_con_opciones_de_seleccion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Codigo,Tipo")] Pregunta_con_opciones_de_seleccion pregunta_con_opciones_de_seleccion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pregunta_con_opciones_de_seleccion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Codigo = new SelectList(db.Pregunta_con_opciones, "Codigo", "TituloCampoObservacion", pregunta_con_opciones_de_seleccion.Codigo);
            return View(pregunta_con_opciones_de_seleccion);
        }

        // GET: Pregunta_con_opciones_de_seleccion/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta_con_opciones_de_seleccion pregunta_con_opciones_de_seleccion = db.Pregunta_con_opciones_de_seleccion.Find(id);
            if (pregunta_con_opciones_de_seleccion == null)
            {
                return HttpNotFound();
            }
            return View(pregunta_con_opciones_de_seleccion);
        }

        // POST: Pregunta_con_opciones_de_seleccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Pregunta_con_opciones_de_seleccion pregunta_con_opciones_de_seleccion = db.Pregunta_con_opciones_de_seleccion.Find(id);
            db.Pregunta_con_opciones_de_seleccion.Remove(pregunta_con_opciones_de_seleccion);
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
