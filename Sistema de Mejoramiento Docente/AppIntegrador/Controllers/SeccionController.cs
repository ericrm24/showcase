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
using AppIntegrador.Utilities;

namespace AppIntegrador.Controllers
{
    public class SeccionController : Controller
    {
        private DataIntegradorEntities db = null;
        public CrearSeccionModel crearSeccion = new CrearSeccionModel();
        private readonly IPerm permissionManager = new PermissionManager();

        public SeccionController()
        {
            this.db = new DataIntegradorEntities();
        }

        public SeccionController(DataIntegradorEntities db)
        {
            this.db = db;
        }

        // GET: Seccion
        [HttpPost]
        public ActionResult ActualizarBancoSecciones(string input0, string input1, string input2)
        {
            if(!permissionManager.IsAuthorized(Permission.VER_SECCION))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            var seccion = db.Seccion;

            ViewBag.filtro = "Ninguno";
            if (input0 == null && input1 == null && input2 == null)
            {
                ViewBag.filtro = "Ninguno";
                return PartialView("_SeccionPartial", seccion.ToList());
            }
            // si se selecionó el código  
            if (input1.Length > 0)
            {
                ViewBag.filtro = "Por código: " + input1;
                //Index action method will return a view with a student records based on what a user specify the value in textbox  
                return PartialView("_SeccionPartial", seccion.Where(x => x.Codigo.Contains(input1)).ToList());
            }
            // si se selecionó el enunciado 
            else if (input2.Length > 0)
            {
                ViewBag.filtro = "Nombre: " + input2;
                return PartialView("_SeccionPartial", seccion.Where(x => x.Nombre.Contains(input2)).ToList());
            }
            else
            {
                ViewBag.filtro = "Ninguno";
                return PartialView("_SeccionPartial", seccion.ToList());
            }
        }

        // GET: Seccion/Create
        public ActionResult Create()
        {
            if(!permissionManager.IsAuthorized(Permission.CREAR_SECCION))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            crearSeccion.pregunta = db.Pregunta;
            return View(crearSeccion);
        }

        // POST: Seccion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Codigo,Nombre")] Seccion seccion, List<Pregunta_con_opciones_de_seleccion> preguntas)
        {
            crearSeccion.pregunta = db.Pregunta;
            if (ModelState.IsValid && seccion.Codigo.Length > 0 && seccion.Nombre.Length > 0)
            {
                if ( InsertSeccionTienePregunta(seccion, preguntas) )
                {
                    return RedirectToAction("Create");
                }
                else
                {
                    // Notifique que ocurrió un error
                    ModelState.AddModelError("Seccion.Codigo", "Código ya en uso.");
                    return View(crearSeccion);
                }
            }

            return View(crearSeccion);
        }


        // GET: Seccion
        public ActionResult Index(string input0, string input1, string input2)
        {
            var seccion = db.Seccion;

            ViewBag.filtro = "Ninguno";
            if (input0 == null && input1 == null && input2 == null)
            {
                ViewBag.filtro = "Ninguno";
                return View(seccion.ToList());
            }
            // si se selecionó el código  
            if (input1.Length > 0)
            {
                ViewBag.filtro = "Por código: " + input1;
                //Index action method will return a view with a student records based on what a user specify the value in textbox  
                return View(seccion.Where(x => x.Codigo.Contains(input1)).ToList());
            }
            // si se selecionó el enunciado 
            else if (input2.Length > 0)
            {
                ViewBag.filtro = "Nombre: " + input2;
                return View(seccion.Where(x => x.Nombre.Contains(input2)).ToList());
            }
            else
            {
                ViewBag.filtro = "Ninguno";
                return View(seccion.ToList());
            }
        }


        // Historia RIP-BKS1
        // Se copió la función para filtrar preguntas.
        [HttpGet]
        public ActionResult Create(string input0, string input1, string input2, string input3)
        {
            if (!permissionManager.IsAuthorized(Permission.CREAR_SECCION))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            crearSeccion.pregunta = db.Pregunta;
            ViewBag.filtro = "Ninguno";
            if (input0 == null && input1 == null && input2 == null && input3 == null)
            {
                crearSeccion.pregunta = db.Pregunta.ToList();
                return View("Create", crearSeccion);
            }
            //if a user choose the radio button option as Subject  
            if (input1.Length > 0)
            {
                ViewBag.filtro = "Por código: " + input1;
                crearSeccion.pregunta = db.Pregunta.Where(x => x.Codigo.Contains(input1)).ToList();
                //Index action method will return a view with a student records based on what a user specify the value in textbox  
            }
            else if (input2.Length > 0)
            {
                ViewBag.filtro = "Enunciado: " + input2;
                crearSeccion.pregunta = db.Pregunta.Where(x => x.Enunciado.Contains(input2)).ToList();
            }
            else if (input3.Length > 0)
            {
                var aux = "";
                switch(input3)
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
                crearSeccion.pregunta = db.Pregunta.Where(x => x.Tipo.Contains(input3)).ToList();
            }
            else
            {
                ViewBag.filtro = "Ninguno";
                crearSeccion.pregunta = db.Pregunta.ToList();
            }
            return View("Create", crearSeccion);
        }

       
        private bool InsertSeccionTienePregunta(Seccion seccion, List<Pregunta_con_opciones_de_seleccion> preguntas)
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

            if (preguntas != null)
            {
                for (int index = 0; index < preguntas.Count; ++index)
                {
                    db.AsociarPreguntaConSeccion(seccion.Codigo, preguntas[index].Codigo);
                }
            }
            return true;
        }

        [HttpGet]
        public ActionResult VistaPrevia(string id)
        {
            if (!permissionManager.IsAuthorized(Permission.VER_SECCION))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Seccion seccion = db.Seccion.Find(id);
            if (seccion == null)
            {
                return HttpNotFound();
            }
            return View(seccion);
        }

        [HttpPost]
        public ActionResult ActualizarCrearSeccion()
        {
            crearSeccion = new CrearSeccionModel();
            return PartialView("~/Views/Seccion/_CreateSeccionPartial.cshtml", crearSeccion);
        }

        public bool AgregarPreguntasASeccion(string codSeccion, List<string> codPreguntas)
        {
            if (codPreguntas != null)
            {
                for (int index = 0; index < codPreguntas.Count; ++index)
                {
                    db.AsociarPreguntaConSeccion(codSeccion, codPreguntas[index]);
                }
            }
            return true;
        }

        [HttpPost]
        public ActionResult AgregarPreguntas(string codSeccion, List<string> codPreguntas)
        {
            return Json(new { insertadoExitoso = AgregarPreguntasASeccion(codSeccion, codPreguntas) });
        }

        public void ObtenerSeccionesConPreguntas(LlenarFormulario formulario, ObjectResult<ObtenerSeccionesDeFormulario_Result> seccionesDeFormulario,
            Respuestas_a_formulario respuestas, bool editar)
        {
            if (formulario != null && seccionesDeFormulario != null)
            {
                foreach (var seccion in seccionesDeFormulario.ToList())
                {
                    List<ObtenerPreguntasDeSeccion_Result> preguntas = db.ObtenerPreguntasDeSeccion(seccion.Codigo).ToList();
                    SeccionConPreguntas nuevaSeccion = new SeccionConPreguntas { CodigoSeccion = seccion.Codigo, Nombre = seccion.Nombre, Preguntas = new List<PreguntaConNumeroSeccion>(), Orden = seccion.Orden };
                    PreguntasController preguntasController = new PreguntasController(this.db);
                    foreach (var pregunta in preguntas)
                    {
                        nuevaSeccion.Preguntas.Add(new PreguntaConNumeroSeccion
                        {
                            Pregunta = new Pregunta { Codigo = pregunta.Codigo, Enunciado = pregunta.Enunciado, Tipo = pregunta.Tipo },
                            OrdenSeccion = nuevaSeccion.Orden,
                            CodigoSeccion = nuevaSeccion.CodigoSeccion,
                            OrdenPregunta = pregunta.Orden,
                            Edit = editar
                        });
                        
                        preguntasController.ObtenerInformacionDePreguntas(nuevaSeccion.Preguntas, nuevaSeccion.CodigoSeccion, respuestas);
                    }
                    formulario.Secciones.Add(nuevaSeccion);
                }
            }
        }

        public List<SeccionConPreguntas> ObtenerSeccionesConPreguntasEditable(string id)
        {
            Formulario formularioDB = db.Formulario.Find(id);
            LlenarFormulario formulario = new LlenarFormulario { Formulario = formularioDB, Secciones = new List<SeccionConPreguntas>() };
            ObjectResult<ObtenerSeccionesDeFormulario_Result> seccionesDeFormulario = db.ObtenerSeccionesDeFormulario(id);

            ObtenerSeccionesConPreguntas(formulario, seccionesDeFormulario, null, true);

            foreach (var seccion in formulario.Secciones)
            {
                seccion.Edicion = true;
            }
            return formulario.Secciones;
        }

        public SeccionConPreguntas ArmarSeccion(string id)
        {
            // Sacar codigo y nombre de la BD
            Seccion secDB = db.Seccion.Find(id);

            if (secDB == null)
            {
                return null;
            }

            SeccionConPreguntas seccion = new SeccionConPreguntas();

            // Asignar datos de la DB al objeto especial
            seccion.CodigoSeccion = secDB.Codigo;
            seccion.Nombre = secDB.Nombre;
            seccion.Orden = 0;

            // Sacar las preguntas y obtener opciones y/o justificaciones
            PreguntasController preguntasController = new PreguntasController(this.db);
            seccion.Preguntas = preguntasController.ArmarPreguntas(seccion);
            preguntasController.ObtenerInformacionDePreguntas(seccion.Preguntas, seccion.CodigoSeccion, null);

            return seccion;
        }

        [HttpGet]
        public ActionResult SeccionConPreguntas(string id)
        {
            if (!permissionManager.IsAuthorized(Permission.VER_SECCION))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }
            // Armar objeto independiente del formulario
            SeccionConPreguntas seccion = ArmarSeccion(id);

            if (seccion == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Meter seccion en una lista por la naturaleza de la vista
            List<SeccionConPreguntas> listaSeccion = new List<SeccionConPreguntas>
            {
                seccion
            };

            return View(listaSeccion);
        }
    }
}
