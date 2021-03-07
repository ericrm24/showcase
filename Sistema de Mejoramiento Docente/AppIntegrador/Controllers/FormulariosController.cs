
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
using System.Data.Entity.Core.Objects;
using System.Threading.Tasks;
using AppIntegrador.Utilities;

namespace AppIntegrador.Controllers
{
    public class FormulariosController : Controller
    {

        private DataIntegradorEntities db;
        public CrearFormularioModel crearFormulario = new CrearFormularioModel();
        private readonly IPerm permissionManager;

        public FormulariosController()
        {
            db = new DataIntegradorEntities();
            permissionManager = new PermissionManager();
        }

        public FormulariosController(DataIntegradorEntities db)
        {
            this.db = db;
            permissionManager = new PermissionManager();
        }


        public PartialViewResult CargarSeccionesFormulario(string Id)
        {
            IEnumerable<ObtenerSeccionesDeFormulario_Result> seccionesSeleccionadas = db.ObtenerSeccionesDeFormulario("CI0128G1").ToList();

            return PartialView("_SeccionesActualesPartial", seccionesSeleccionadas);
        }

        public ActionResult DesplegarFormulario(string id)
        {
            SeccionController seccionController = new SeccionController();
            var result = seccionController.ObtenerSeccionesConPreguntasEditable(id);
            seccionController.Dispose();
            return PartialView("../Seccion/SeccionConPreguntas", result);
        }

        [HttpPost]
        public bool BorrarSeccion(string FCodigo, string SCodigo)
        {
            if (FCodigo != null && SCodigo != null)
            {
                try
                {
                    if (db.EliminarSeccionFormulario(FCodigo, SCodigo) == 0)
                    {
                        return false;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        [HttpPost]
        public bool BorrarPregunta(string SCodigo, string PCodigo)
        {
            if (SCodigo != null && PCodigo != null)
            {
                try
                {
                    if (db.EliminarPreguntaDeSeccion(SCodigo, PCodigo) == 0)
                    {
                        return false;
                    }
                }
                catch (System.Data.Entity.Core.EntityCommandExecutionException)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        // Retorna la vista "parcial" de pregunta Respuesta libre (.cshtml)
        public ActionResult RespuestaLibre()
        {
            ViewBag.message = "RespuestaLibre";
            return View("RespuestaLibre");
        }

        // GET: Formularios
        public ActionResult Index(string input0, string input1, string input2)
        {
            if (!permissionManager.IsAuthorized(Permission.CREAR_FORMULARIO))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }
            var formulario = db.Formulario;

            ViewBag.filtro = "Ninguno";
            if (input0 == null && input1 == null && input2 == null)
            {
                ViewBag.filtro = "Ninguno";
                return View("Index", formulario.ToList());
            }
            // si se selecionó el código  
            if (input1.Length > 0)
            {
                ViewBag.filtro = "Por código: " + input1;
                //Index action method will return a view with a student records based on what a user specify the value in textbox  
                return View("Index", formulario.Where(x => x.Codigo.Contains(input1)).ToList());
            }
            // si se selecionó el enunciado 
            else if (input2.Length > 0)
            {
                ViewBag.filtro = "Nombre: " + input2;
                return View("Index", formulario.Where(x => x.Nombre.Contains(input2)).ToList());
            }
            else
            {
                ViewBag.filtro = "Ninguno";
                return View("Index", formulario.ToList());
            }
        }

        // GET: Formularios/Details/5
        public ActionResult Details(string id)
        {
            if (!permissionManager.IsAuthorized(Permission.VER_DETALLES_FORMULARIO))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            crearFormulario.seccion = db.Seccion;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formulario formulario = db.Formulario.Find(id);
            if (formulario == null)
            {
                return HttpNotFound();
            }
            return View(formulario);
        }

        // GET: Formularios/Create
        public ActionResult Create()
        {
            return CrearFormularioView(null, false);
        }

        public ActionResult Edit(string codForm)
        {
            return CrearFormularioView(codForm, true);
        }

        public ActionResult CrearFormularioView(string codForm, bool creado)
        {
            if (!permissionManager.IsAuthorized(Permission.CREAR_FORMULARIO))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }
            
            if(creado)
            {
                crearFormulario.Formulario = db.Formulario.Find(codForm);
                if(crearFormulario.Formulario == null)
                {
                    return RedirectToAction("Index");
                }

                SeccionController seccionController = new SeccionController();
                crearFormulario.seccionesConPreguntas = seccionController.ObtenerSeccionesConPreguntasEditable(codForm);
                seccionController.Dispose();
            }
            else
            {
                crearFormulario.Formulario = new Formulario();
            }
            crearFormulario.seccion = db.Seccion;
            crearFormulario.crearSeccionModel = new CrearSeccionModel();
            crearFormulario.Creado = creado;
            ViewBag.Version = "Creacion";
            return View("Create", crearFormulario);
        }

        // POST: Formularios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Codigo,Nombre")] Formulario formulario, int? formularioCreado)
        {
            crearFormulario.seccion = db.Seccion;
            crearFormulario.crearSeccionModel = new CrearSeccionModel();
            crearFormulario.Formulario = formulario;
            if (formulario != null)
            {
                SeccionController seccionController = new SeccionController();
                crearFormulario.seccionesConPreguntas = seccionController.ObtenerSeccionesConPreguntasEditable(formulario.Codigo);
                seccionController.Dispose();
            }
            ViewBag.Version = "Creacion";
            if (formularioCreado == 1)
            {
                ViewBag.Message = "Exitoso";
                crearFormulario.Creado = true;
                return View(crearFormulario);
            }
            if (ModelState.IsValid && formulario.Codigo.Length > 0 && formulario.Nombre.Length > 0)
            {
                if (InsertFormulario(formulario))
                {
                    crearFormulario.Creado = true;
                    ViewBag.Message = "Exitoso";
                    return View(crearFormulario);
                }
                else
                {
                    // Notifique que ocurrió un error
                    ViewBag.Message = "Fallido";
                    crearFormulario.Formulario = formulario;
                    crearFormulario.crearSeccionModel = new CrearSeccionModel();
                    return View(crearFormulario);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AgregarSeccion(Seccion seccion)
        {
            return Json(new { guardadoExitoso = seccion != null && InsertSeccion(seccion) });
        }

        // Historia RIP-CF5
        // Se copió la función para filtrar preguntas.
        //        [HttpPost]
        public ActionResult AplicarFiltro(string input0, string input1, string input2)
        {
            crearFormulario.seccion = db.Seccion;

            ViewBag.filtro = "Ninguno";
            if (input0 == null && input1 == null && input2 == null)
            {
                crearFormulario.seccion = db.Seccion.ToList();
                return PartialView("~/Views/Seccion/_SeccionPartial.cshtml", crearFormulario.seccion);
            }
            //if a user choose the radio button option as Subject  
            if (input1.Length > 0)
            {
                ViewBag.filtro = "Por código: " + input1;
                crearFormulario.seccion = db.Seccion.Where(x => x.Codigo.Contains(input1)).ToList();
                //Index action method will return a view with a student records based on what a user specify the value in textbox  
            }
            else if (input2.Length > 0)
            {
                ViewBag.filtro = "Nombre: " + input2;
                crearFormulario.seccion = db.Seccion.Where(x => x.Nombre.Contains(input2)).ToList();
            }
            else
            {
                ViewBag.filtro = "Ninguno";
                crearFormulario.seccion = db.Seccion.ToList();
            }
            return PartialView("~/Views/Seccion/_SeccionPartial.cshtml", crearFormulario.seccion);
        }
        /**
         * Este método valida si ya el formulario fue creado, de no ser así
         * lo crea y le asocia las secciones recibidas por parámetros
         * 
         */
        [HttpPost]
        public ActionResult AsociarSeccionesAFormulario(string codigo, string nombre, List<string> seccionesAsociadas)
        {
            Formulario form = new Formulario();
            form.Codigo = codigo;
            form.Nombre = nombre;

            if (ModelState.IsValid && codigo != null && nombre != null && codigo.Length > 0 && nombre.Length > 0)
            {
                if (InsertFormularioTieneSeccion(form, seccionesAsociadas))
                {
                    ViewBag.Message = "Exitoso";
                }
                else
                {
                    // Notifique que ocurrió un error
                    ModelState.AddModelError("Formulario.Codigo", "Código ya en uso.");
                }
            }

            return DesplegarFormulario(form.Codigo);
        }



        // GET: Formularios/Delete/5
        public ActionResult Delete(string id)
        {
            if (!permissionManager.IsAuthorized(Permission.BORRAR_FORMULARIO))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            crearFormulario.seccion = db.Seccion;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formulario formulario = db.Formulario.Find(id);
            if (formulario == null)
            {
                return HttpNotFound();
            }
            return View(formulario);
        }

        // POST: Formularios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            crearFormulario.seccion = db.Seccion;
            Formulario formulario = db.Formulario.Find(id);
            db.Formulario.Remove(formulario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            crearFormulario.seccion = db.Seccion;
            if (disposing)
            {
                db.Dispose();
            }
            crearFormulario.seccion = db.Seccion;
            base.Dispose(disposing);
        }

        private bool InsertSeccion(Seccion seccion)
        {
            try
            {
                if (db.AgregarSeccion(seccion.Codigo, seccion.Nombre) == 0)
                {
                    return false;
                }
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException)
            {
                return false;
            }
            return true;
        }
        
        private bool InsertFormularioTieneSeccion(Formulario formulario, List<String> secciones)
        {
            if (formulario == null || secciones == null)
            {
                return false;
            }

            for (int index = 0; index < secciones.Count; ++index)
            {
                db.AsociarSeccionConFormulario(formulario.Codigo, secciones[index]);
            }
            return true;
        }

        private bool InsertFormulario(Formulario formulario)
        {
            if(formulario == null || formulario.Codigo == null || formulario.Nombre == null)
            {
                return false;
            }
            try
            {
                if (db.AgregarFormulario(formulario.Codigo, formulario.Nombre) == 0)
                {
                    return false;
                }
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException)
            {
                return false;
            }
            return true;
        }


        [HttpPost]
        public ActionResult AgregarFormulario(Formulario formulario)
        {
            return Json(new { guardadoExitoso = formulario != null && InsertFormulario(formulario) });
        }

        [HttpPost]
        public ActionResult EliminarSeccion(string FCodigo, string SCodigo)
        {
            if (FCodigo != null && SCodigo != null)
            {
                return Json(new { eliminadoExitoso = BorrarSeccion(FCodigo, SCodigo) });
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult EliminarPregunta(string SCodigo, string PCodigo)
        {
            if (SCodigo != null && PCodigo != null)
            {
                return Json(new { eliminadoExitoso = BorrarPregunta(SCodigo, PCodigo) });
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult ModificarFormulario(string codViejo, string codNuevo, string nombre)
        {
            bool modificacionExitosa = false;
            if(codViejo != null && codNuevo != null && nombre != null)
            {
                ObjectParameter modificacionOutput = new ObjectParameter("modificacionexitosa", typeof(bool));
                db.ModificarFormulario(codViejo, codNuevo, nombre, modificacionOutput);
                modificacionExitosa = (bool)modificacionOutput.Value;
            }
            return Json(new { modificacionExitosa });
        }
    }
}


