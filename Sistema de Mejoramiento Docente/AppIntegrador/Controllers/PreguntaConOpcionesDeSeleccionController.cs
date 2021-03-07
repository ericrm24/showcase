
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AppIntegrador.Models;
using System.Data.SqlClient;
using System.Configuration;
using AppIntegrador.Utilities;

namespace AppIntegrador.Controllers
{
    public class PreguntaConOpcionesDeSeleccionController : Controller
    {
        private DataIntegradorEntities db = new DataIntegradorEntities();

        private readonly IPerm permissionManager = new PermissionManager();

        [HttpGet]
        public ActionResult Pregunta_con_opciones_de_seleccion()
        {
            return View();
        }

        [HttpGet]
        public ActionResult OpcionesDeSeleccion(int? i)
        {
            if (!permissionManager.IsAuthorized(Permission.VER_PREGUNTAS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }
            ViewBag.i = i;
            return PartialView();
        }

        // GET: PreguntaConOpcionesDeSeleccion
        //public ActionResult Index()
        //{
        //    var pregunta_con_opciones_de_seleccion = db.Pregunta_con_opciones_de_seleccion;
        //    return View(pregunta_con_opciones_de_seleccion.ToList());
        //}

        //the first parameter is the option that we choose and the second parameter will use the textbox value  
        public ActionResult Index(string input0, string input1, string input2, string input3)
        {
            if (!permissionManager.IsAuthorized(Permission.VER_PREGUNTAS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }
            var pregunta = db.Pregunta;

            ViewBag.filtro = "Ninguno";
            if (input0 == null && input1 == null && input2 == null && input3 == null)
            {
                ViewBag.filtro = "Ninguno";
                return View(pregunta.ToList());
            }
            // si se selecionó el código  
            if (input1.Length > 0)
            {
                ViewBag.filtro = "Por código: " + input1;
                //Index action method will return a view with a student records based on what a user specify the value in textbox  
                return View(pregunta.Where(x => x.Codigo.Contains(input1)).ToList());
            }
            // si se selecionó el enunciado 
            else if ( input2.Length > 0)
            {
                ViewBag.filtro = "Enunciado: " + input2;
                return View(pregunta.Where(x => x.Enunciado.Contains(input2)).ToList());
            }
            // si se seleccionó el tipo
            else if (input3.Length > 0)
            {
                var aux = "";
                switch (input3)
                {
                    case "U":
                        aux = "Selección Única";
                        break;
                    case "M":
                        aux = "Selección Múltiple";
                        break;
                    case "L":
                        aux = "Respuesta libre";
                        break;
                    case "S":
                        aux = "Sí/No/NR";
                        break;
                    case "E":
                        aux = "Escalar";
                        break;
                    default:
                        break;
                }
                ViewBag.filtro = "Tipo: " + aux;
                return View(pregunta.Where(x => x.Tipo.Contains(input3)).ToList());
            }
            else
            {
                ViewBag.filtro = "Ninguno";
                return View(pregunta.ToList());
            }
        }
        [HttpPost]
        public ActionResult ActualizarBancoPreguntas(string input0, string input1, string input2, string input3)
        {
            var pregunta = db.Pregunta;

            ViewBag.filtro = "Ninguno";
            if (input0 == null && input1 == null && input2 == null && input3 == null)
            {
                ViewBag.filtro = "Ninguno";
                return PartialView(pregunta.ToList());
            }
            // si se selecionó el código  
            if (input1.Length > 0)
            {
                ViewBag.filtro = "Por código: " + input1;
                //Index action method will return a view with a student records based on what a user specify the value in textbox  
                return PartialView(pregunta.Where(x => x.Codigo.Contains(input1)).ToList());
            }
            // si se selecionó el enunciado 
            else if ( input2.Length > 0)
            {
                ViewBag.filtro = "Enunciado: " + input2;
                return PartialView(pregunta.Where(x => x.Enunciado.Contains(input2)).ToList());
            }
            // si se seleccionó el tipo
            else if (input3.Length > 0)
            {
                var aux = "";
                switch (input3)
                {
                    case "U":
                        aux = "Selección Única";
                        break;
                    case "M":
                        aux = "Selección Múltiple";
                        break;
                    case "L":
                        aux = "Respuesta libre";
                        break;
                    case "S":
                        aux = "Sí/No/NR";
                        break;
                    case "E":
                        aux = "Escalar";
                        break;
                    default:
                        break;
                }
                ViewBag.filtro = "Tipo: " + aux;
                return PartialView(pregunta.Where(x => x.Tipo.Contains(input3)).ToList());
            }
            else
            {
                ViewBag.filtro = "Ninguno";
                return PartialView(pregunta.ToList());
            }
        }




        // GET: PreguntaConOpcionesDeSeleccion/Details/5
        public ActionResult Details(string id)
        {
            if (!permissionManager.IsAuthorized(Permission.VER_DETALLES_PREGUNTA))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

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

        // GET: PreguntaConOpcionesDeSeleccion/Create
        // Metodo usado para el render partial
        public ActionResult OpcionesDeSeleccion()
        {
            return View("OpcionesSeleccion");
        }

        public ActionResult Create()
        {
            if (!permissionManager.IsAuthorized(Permission.CREAR_PREGUNTA))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }
            return View();
        }

        // POST: PreguntaConOpcionesDeSeleccion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pregunta_con_opciones_de_seleccion pregunta, List<Opciones_de_seleccion> Opciones)
        {
            if (ModelState.IsValid && pregunta.Codigo.Length > 0 && pregunta.Pregunta_con_opciones.Pregunta.Enunciado.Length > 0)
            {
                bool validOptions = Opciones != null;
                if (validOptions)
                {
                    validOptions = false;
                    foreach (Opciones_de_seleccion opcion in Opciones)
                    {
                        if (opcion.Texto != null && opcion.Texto.Length > 0)
                        {
                            validOptions = true;
                        }
                    }
                }

                if(!validOptions)
                {
                    ModelState.AddModelError("", "Una pregunta de selección única necesita al menos una opción");
                    return View(pregunta);
                }
                try
                {
                    if(db.AgregarPreguntaConOpcion(pregunta.Codigo, "U", pregunta.Pregunta_con_opciones.Pregunta.Enunciado, pregunta.Pregunta_con_opciones.TituloCampoObservacion, null) == 0)
                    {
                        ModelState.AddModelError("Codigo", "Código ya en uso.");
                        return View(pregunta);
                    }
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    ModelState.AddModelError("Codigo", "Código ya en uso.");
                    return View(pregunta);
                }

                foreach (Opciones_de_seleccion opcion in Opciones)
                {
                    db.AgregarOpcion(pregunta.Codigo, (byte)opcion.Orden, opcion.Texto);
                }

                    ViewBag.Message = "Exitoso";
                    return View();
            }

            return View();
        }

        // GET: PreguntaConOpcionesDeSeleccion/Edit/5
        public ActionResult Edit(string id)
        {
            if (!permissionManager.IsAuthorized(Permission.EDITAR_PREGUNTA))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

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

        // POST: PreguntaConOpcionesDeSeleccion/Edit/5
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

        // GET: PreguntaConOpcionesDeSeleccion/Delete/5
        public ActionResult Delete(string id)
        {
            if (!permissionManager.IsAuthorized(Permission.BORRAR_PREGUNTA))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

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

        // POST: PreguntaConOpcionesDeSeleccion/Delete/5
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
