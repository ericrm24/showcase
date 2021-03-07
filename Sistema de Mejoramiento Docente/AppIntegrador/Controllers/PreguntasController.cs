using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AppIntegrador.Models;
using AppIntegrador.Utilities;

namespace AppIntegrador.Controllers
{
    public class PreguntasController : Controller
    {
        private DataIntegradorEntities db;
        private DataIntegradorEntities @object;
        private readonly IPerm permissionManager;

        public PreguntasController()
        {
            this.db = new DataIntegradorEntities();
            this.permissionManager = new PermissionManager();
        }

        public PreguntasController(DataIntegradorEntities db)
        {
            this.db = db;
            this.permissionManager = new PermissionManager();
        }

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
            else if (input2.Length > 0)
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
        public ActionResult ActualizarBancoPreguntas(string input0 = null, string input1 = null, string input2 = null, string input3 = null)
        {
            var pregunta = db.Pregunta;

            ViewBag.filtro = "Ninguno";
            if (input0 == null && input1 == null && input2 == null && input3 == null)
            {
                ViewBag.filtro = "Ninguno";
                return PartialView("~/Views/PreguntaConOpcionesDeSeleccion/_IndexPartial.cshtml", pregunta.ToList());
            }
            // si se selecionó el código  
            if (input1.Length > 0)
            {
                ViewBag.filtro = "Por código: " + input1;
                //Index action method will return a view with a student records based on what a user specify the value in textbox  
                return PartialView("~/Views/PreguntaConOpcionesDeSeleccion/_IndexPartial.cshtml", pregunta.Where(x => x.Codigo.Contains(input1)).ToList());
            }
            // si se selecionó el enunciado 
            else if (input2.Length > 0)
            {
                ViewBag.filtro = "Enunciado: " + input2;
                return PartialView("~/Views/PreguntaConOpcionesDeSeleccion/_IndexPartial.cshtml", pregunta.Where(x => x.Enunciado.Contains(input2)).ToList());
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
                return PartialView("~/Views/PreguntaConOpcionesDeSeleccion/_IndexPartial.cshtml", pregunta.Where(x => x.Tipo.Contains(input3)).ToList());
            }
            else
            {
                ViewBag.filtro = "Ninguno";
                return PartialView("~/Views/PreguntaConOpcionesDeSeleccion/_IndexPartial.cshtml", pregunta.ToList());
            }
        }

        // GET: Preguntas/Create
        public ActionResult Create()
        {
            if (!permissionManager.IsAuthorized(Permission.CREAR_PREGUNTA))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }
            ViewBag.message = "Crear pregunta";
            return View("Create");
        }

        public ActionResult CreateBase()
        {
            ViewBag.message = "Crear pregunta";
            return PartialView("Create");
        }

        public ActionResult GuardarRespuestaLibre(Pregunta pregunta)
        {
            // asegurarse que exista la preguna
            if (pregunta != null)
            {
                try
                {
                    // se trata de guardar la pregunta de respuesta libre 
                    if (db.AgregarPreguntaRespuestaLibre(pregunta.Codigo, "L", pregunta.Enunciado) == 0)
                    {
                        // si se presentó un problema, se devuelve el codigo de error
                        ModelState.AddModelError("Codigo", "Código ya en uso.");
                        return Json(new { guardadoExitoso = false, DatosIncompletos = false, CodigoEnUso = true, FaltaOpciones = false, MinMax = false });
                    }
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    // si se presentó un problema, se devuelve el codigo de error
                    ModelState.AddModelError("Codigo", "Código ya en uso.");
                    return Json(new { guardadoExitoso = false, DatosIncompletos = false, CodigoEnUso = true, FaltaOpciones = false, MinMax = false });
                }
            }

            ViewBag.message = "Crear pregunta";
            return Json(new { guardadoExitoso = true, DatosIncompletos = false, CodigoEnUso = false, FaltaOpciones = false, MinMax = false });
        }

        public ActionResult GuardarPreguntaSiNo(Pregunta pregunta)
        {
            // asegurarse que exista la preguna
            if (pregunta != null)
            {
                try
                {
                    ObjectParameter returnValue = new ObjectParameter("NumeroError", typeof(int));
                    // se trata de guardar la pregunta de con opciones de si/no/nr
                    if (db.AgregarPreguntaConOpcion(pregunta.Codigo, "S", pregunta.Enunciado, pregunta.Pregunta_con_opciones.TituloCampoObservacion, returnValue) == 0)
                    {
                        // si se presentó un problema, se devuelve el codigo de error
                        ModelState.AddModelError("Codigo", "Código ya en uso.");
                        return Json(new { guardadoExitoso = false, DatosIncompletos = false, CodigoEnUso = true, FaltaOpciones = false, MinMax = false });
                    }
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    // si se presentó un problema, se devuelve el codigo de error
                    ModelState.AddModelError("Codigo", "Código ya en uso.");
                    return Json(new { guardadoExitoso = false, DatosIncompletos = false, CodigoEnUso = true, FaltaOpciones = false, MinMax = false });
                }
            }

            ViewBag.message = "Crear pregunta";
            return Json(new { guardadoExitoso = true, DatosIncompletos = false, CodigoEnUso = false, FaltaOpciones = false, MinMax = false });
        }

        public ActionResult GuardarPreguntaEscalar(Pregunta pregunta, int min, int max)
        {
            // asegurarse que exista la preguna
            if (pregunta != null)
            {
                if (max > min)
                {
                    try
                    {
                        ObjectParameter returnValue = new ObjectParameter("NumeroError", typeof(int));
                        // se trata de guardar la pregunta de con opciones de
                        if (db.AgregarPreguntaEscalar(pregunta.Codigo, "E", pregunta.Enunciado, pregunta.Pregunta_con_opciones.TituloCampoObservacion, 1, min, max, returnValue) == 0)
                        {
                            // si se presentó un problema, se devuelve el codigo de error
                            ModelState.AddModelError("Codigo", "Código ya en uso.");
                            return Json(new { guardadoExitoso = false, DatosIncompletos = false, CodigoEnUso = true, FaltaOpciones = false, MinMax = false });
                        }
                    }
                    catch (System.Data.SqlClient.SqlException)
                    {
                        // si se presentó un problema, se devuelve el codigo de error
                        ModelState.AddModelError("Codigo", "Código ya en uso.");
                        return Json(new { guardadoExitoso = false, DatosIncompletos = false, CodigoEnUso = true, FaltaOpciones = false, MinMax = false });
                    }
                }
                else
                {
                    // si está intentando poner un rango inválido
                    ModelState.AddModelError("min", "El mínimo debe ser menor al máximo");
                    return Json(new { guardadoExitoso = false, DatosIncompletos = false, CodigoEnUso = false, FaltaOpciones = false, MinMax = true });
                }
            }

            ViewBag.message = "Crear pregunta";
            return Json(new { guardadoExitoso = true, DatosIncompletos = false, CodigoEnUso = false, FaltaOpciones = false, MinMax = false });
        }


        [HttpPost]
        public ActionResult GuardarPregunta(Pregunta pregunta, string Justificacion,  List<string> TextoOpciones, int min = 0, int max = 0)
        {
            // Se fija que la pregunta no sea nula y que tenga opciones, a menos que sea escalar o libre, que no requieren opciones
            if (pregunta == null || (TextoOpciones == null && pregunta.Tipo == "U" && pregunta.Tipo == "M"))
            {
                ModelState.AddModelError("", "Datos incompletos");
                return Json(new { guardadoExitoso = false, DatosIncompletos = true, CodigoEnUso = false, FaltaOpciones = false, MinMax = false}) ;
            }

            if (Justificacion == "")
            {
                Justificacion = null;
            }

            List<Opciones_de_seleccion> Opciones = new List<Opciones_de_seleccion>();
            if(TextoOpciones != null)
            {
                for(int count = 0; count < TextoOpciones.Count; ++count)
                {
                    Opciones_de_seleccion opcion = new Opciones_de_seleccion();
                    opcion.Codigo = pregunta.Codigo;
                    opcion.Orden = count;
                    opcion.Texto = TextoOpciones[count];
                    Opciones.Add(opcion);
                }
            }

            if (ModelState.IsValid && pregunta.Codigo.Length > 0 && pregunta.Enunciado.Length > 0)
            {
                // GuardarPreguntaSiNo las preguntas dependiendo del tipo
                // ToDo: realizar un método para cada tipo de pregunta
                switch (pregunta.Tipo)
                {
                    case "U":
                        pregunta.Pregunta_con_opciones = new Pregunta_con_opciones();
                        break;
                    case "M":
                        pregunta.Pregunta_con_opciones = new Pregunta_con_opciones();
                        break;
                    case "L": return GuardarRespuestaLibre(pregunta);
                    case "S":
                        pregunta.Pregunta_con_opciones = new Pregunta_con_opciones();
                        return GuardarPreguntaSiNo(pregunta);
                    case "E":
                        pregunta.Pregunta_con_opciones = new Pregunta_con_opciones();
                        return GuardarPreguntaEscalar(pregunta, min, max);
                }

                pregunta.Pregunta_con_opciones.TituloCampoObservacion = Justificacion;

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

                if (!validOptions)
                {
                    ModelState.AddModelError("", "Una pregunta de selección única necesita al menos una opción");
                    return Json(new { guardadoExitoso = false, DatosIncompletos = false, CodigoEnUso = false, FaltaOpciones = true, MinMax = false}) ;                    
                }
                try
                {
                    ObjectParameter returnValue = new ObjectParameter("NumeroError", typeof(int));
                    if (db.AgregarPreguntaConOpcion(pregunta.Codigo, pregunta.Tipo, pregunta.Enunciado, pregunta.Pregunta_con_opciones.TituloCampoObservacion,returnValue) == 0)
                    {
                        ModelState.AddModelError("Codigo", "Código ya en uso.");                        
                        return Json(new { guardadoExitoso = false, DatosIncompletos = false, CodigoEnUso = true, FaltaOpciones = false, MinMax = false}) ;                    
                    }
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    ModelState.AddModelError("Codigo", "Código ya en uso.");
                    return Json(new { guardadoExitoso = false, DatosIncompletos = false, CodigoEnUso = true, FaltaOpciones = false, MinMax = false}) ;                                        
                }

                foreach (Opciones_de_seleccion opcion in Opciones)
                {
                    db.AgregarOpcion(pregunta.Codigo, (byte)opcion.Orden, opcion.Texto);
                }

                ModelState.Clear();
                ViewBag.Message = "Exitoso";
                return Json(new { guardadoExitoso = true, DatosIncompletos = false, CodigoEnUso = false, FaltaOpciones = false, MinMax = false}) ;            
            }
            else
            {
                ModelState.AddModelError("", "Datos incompletos");
                return Json(new { guardadoExitoso = false, DatosIncompletos = true, CodigoEnUso = false, FaltaOpciones = false, MinMax = false}) ;            
            }
        }

        // Retorna la vista "parcial" de Respuesta libre (.cshtml)
        [HttpPost]
        public ActionResult RespuestaLibre()
        {
            ViewBag.message = "Respuesta Libre";
            return View("RespuestaLibre");
        }

        // Retorna la vista "parcial" de Pregunta Escalar (.cshtml)
        [HttpPost]
        public ActionResult PreguntaEscalar()
        {
            ViewBag.message = "Pregunta Escalar";
            return View("PreguntaEscalar");
        }

        // Retorna la vista "parcial" de pregunta Si/No/NR (.cshtml)
        [HttpPost]
        public ActionResult PreguntaSiNo()
        {
            ViewBag.message = "Pregunta Si/No/NR";
            return View("PreguntaSiNo");
        }

        // Retorna la vista "parcial" de pregunta con opciones (.cshtml)
        [HttpPost]
        public ActionResult PreguntaConOpciones()
        {
            ViewBag.message = "Pregunta con opciones";
            return View("PreguntaConOpciones");

        }

        [HttpPost]
        public ActionResult OpcionesDeSeleccion(int i, char Tipo)
        {
            if (!permissionManager.IsAuthorized(Permission.VER_PREGUNTAS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            if (i < 0)
            {
                return null;
            }
            ViewBag.Tipo = Tipo;
            ViewBag.i = i;
            return View("OpcionesDeSeleccion");
        }

        [HttpGet]
        public ActionResult VistaPrevia(string id)
        {
            if (!permissionManager.IsAuthorized(Permission.VER_PREGUNTAS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pregunta pregunta = db.Pregunta.Find(id);
            if (pregunta == null)
            {
                return HttpNotFound();
            }
            return View(pregunta);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpGet]
        public ActionResult Estilos()
        {
            ViewBag.message = "Estilos UCR";
            return View("Estilos");
        }

        public void ObtenerInformacionDePreguntas(IEnumerable<PreguntaConNumeroSeccion> preguntas, string codSeccion, Respuestas_a_formulario respuestas)
        {
            if (preguntas != null)
            {
                foreach (PreguntaConNumeroSeccion pregunta in preguntas)
                {
                    if (pregunta.Pregunta.Tipo == "U" || pregunta.Pregunta.Tipo == "M" || pregunta.Pregunta.Tipo == "E" || pregunta.Pregunta.Tipo == "S")
                    {
                        pregunta.Pregunta.Pregunta_con_opciones = db.Pregunta_con_opciones.Find(pregunta.Pregunta.Codigo);
                        if (respuestas != null)
                        {
                            var resultadoRespuestaGuardada = db.ObtenerRespuestasAPreguntaConOpciones(respuestas.FCodigo, respuestas.Correo, respuestas.CSigla, respuestas.GNumero, respuestas.GSemestre, respuestas.GAnno,
                                                                                     codSeccion, pregunta.Pregunta.Codigo);
                            if (resultadoRespuestaGuardada != null)
                            {
                                var respuestaGuardada = resultadoRespuestaGuardada.ToList();
                                if (respuestaGuardada.Any())
                                {
                                    pregunta.RespuestaLibreOJustificacion = respuestaGuardada.FirstOrDefault().Justificacion;

                                    var opcionesGuardadas = db.ObtenerOpcionesSeleccionadas(respuestas.FCodigo, respuestas.Correo, respuestas.CSigla, respuestas.GNumero, respuestas.GSemestre, respuestas.GAnno,
                                                                                         codSeccion, pregunta.Pregunta.Codigo);
                                    pregunta.Opciones = new List<int>();
                                    if (opcionesGuardadas != null)
                                    {
                                        foreach (var opcion in opcionesGuardadas.ToList())
                                        {
                                            pregunta.Opciones.Add(opcion.OpcionSeleccionada);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (pregunta.Pregunta.Tipo == "L" && respuestas != null)
                    {
                        var respuestaGuardada = db.ObtenerRespuestaLibreGuardada(respuestas.FCodigo, respuestas.Correo, respuestas.CSigla,
                                                                    respuestas.GNumero, respuestas.GAnno, respuestas.GSemestre, pregunta.Pregunta.Codigo, codSeccion).ToList();
                        if (respuestaGuardada.Any())
                        {
                            pregunta.RespuestaLibreOJustificacion = respuestaGuardada.FirstOrDefault().Observacion;
                        }
                    }
                }
            }
        }

        public List<PreguntaConNumeroSeccion> ArmarPreguntas(SeccionConPreguntas seccion)
        {
            List<PreguntaConNumeroSeccion> listaPreguntas = new List<PreguntaConNumeroSeccion>();

            // Sacar preguntas con el procedimiento almacenado
            List<ObtenerPreguntasDeSeccion_Result> preguntas = db.ObtenerPreguntasDeSeccion(seccion.CodigoSeccion).ToList();

            // Poblar la lista de preguntas segun lo obtenido del procedimiento
            foreach (var pregunta in preguntas)
            {
                listaPreguntas.Add(new PreguntaConNumeroSeccion
                {
                    Pregunta = new Pregunta { Codigo = pregunta.Codigo, Enunciado = pregunta.Enunciado, Tipo = pregunta.Tipo },
                    OrdenSeccion = seccion.Orden,
                    OrdenPregunta = pregunta.Orden
                });
            }

            return listaPreguntas;
        }

        [HttpGet]
        public ActionResult TodasLasPreguntas(string id)
        {
            if (!permissionManager.IsAuthorized(Permission.VER_PREGUNTAS))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            Pregunta pregDB = db.Pregunta.Find(id);
            if (pregDB == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PreguntaConNumeroSeccion pregTmp = new PreguntaConNumeroSeccion
            {
                Pregunta = pregDB,
            };
            List<PreguntaConNumeroSeccion> listaPregunta = new List<PreguntaConNumeroSeccion>();
            listaPregunta.Add(pregTmp);

            ObtenerInformacionDePreguntas(listaPregunta, null, null);

            return View("../ResponderPreguntas/TodasLasPreguntas", listaPregunta);
        }
    }
}





