using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LINQtoCSV;
using System.Data.Entity.Core.Objects;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.IO;
using AppIntegrador.Models;
using System.Text.RegularExpressions;
using System.Diagnostics;
using AppIntegrador.Utilities;

namespace AppIntegrador.Controllers
{
    public class CSVController : Controller
    {

        private DataIntegradorEntities db = new DataIntegradorEntities();
        private readonly IPerm permissionManager = new PermissionManager();

        public ActionResult Index()
        {
            if (!permissionManager.IsAuthorized(Permission.CARGAR_DATOS_DESDE_CSV))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            return View(); 
        }

        public ActionResult Clase()
        {
            if (!permissionManager.IsAuthorized(Permission.CARGAR_DATOS_DESDE_CSV))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            return View();
        }

        public ActionResult GuiaHorarios()
        {
            if (!permissionManager.IsAuthorized(Permission.CARGAR_DATOS_DESDE_CSV))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            return View();
        }

        public ActionResult Funcionarios()
        {
            if (!permissionManager.IsAuthorized(Permission.CARGAR_DATOS_DESDE_CSV))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            bool error;
            if (file != null && file.ContentLength > 0)
            {//Archivo no es nulo o vacío
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Estudiantes"), //Server mapPath contiene el path del proyecto + la carpeta ArchivoCSV que es donde va el archivo
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    error = cargarListaEstudiante(path).Item1;
                    if (!error)
                    {
                        ViewBag.Message = "Fallido";
                        ViewBag.Campo = cargarListaEstudiante(path).Item2;
                    }
                    else
                    {
                        ViewBag.Message = "Exitoso";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "Por favor especifique un archivo";
            }

            return View();
        }

        [HttpPost]
        public ActionResult Clase(HttpPostedFileBase file)
        {
            bool error;
            if (file != null && file.ContentLength > 0)
            { //Archivo no es nulo o vacío
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Listas de Clase"), //Server mapPath contiene el path del proyecto + la carpeta ArchivoCSV que es donde va el archivo
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    error = cargarListaClase(path).Item1;
                    if (!error)
                    {
                        ViewBag.Message = "Fallido"; //TO-DO: Debe cambiarse por llamados a validaciones
                        ViewBag.Campo = cargarListaClase(path).Item2;

                    }
                    else
                    {
                        ViewBag.Message = "Exitoso";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "Por favor especifique un archivo";
            }

            return View();
        }

        [HttpPost]
        public ActionResult Funcionarios(HttpPostedFileBase file)
        {
            bool error;
            if (file != null && file.ContentLength > 0)
            {//Archivo no es nulo o vacío
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Funcionarios"), //Server mapPath contiene el path del proyecto + la carpeta ArchivoCSV que es donde va el archivo
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    error = cargarListaFuncionario(path).Item1;
                    if (!error)
                    {
                        ViewBag.Message = "Fallido";
                        ViewBag.Campo = cargarListaFuncionario(path).Item2;
                    }
                    else
                    {
                        ViewBag.Message = "Exitoso";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "Fallido";
                    ViewBag.Campo = ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "Por favor especifique un archivo";
            }

            return View();
        }

        [HttpPost]
        public ActionResult GuiaHorarios(HttpPostedFileBase file)
        {
            bool error;
            if (file != null && file.ContentLength > 0)
            {//Archivo no es nulo o vacío
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Guias Horario"), //Server mapPath contiene el path del proyecto + la carpeta ArchivoCSV que es donde va el archivo
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    error = cargarGuia(path).Item1;
                    if (!error)
                    {
                        ViewBag.Message = "Fallido";
                        ViewBag.Campo = cargarGuia(path).Item2;
                    }
                    else
                    {
                        ViewBag.Message = "Exitoso";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "Por favor especifique un archivo";
            }

            return View();
        }

        public Tuple<bool, string> cargarListaClase(string path)
        {
            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',', //Indica qué es lo que separa cada valor en el archivo
                FirstLineHasColumnNames = true, //La primera fila corresponde a los títulos de los campos, no a un campo específico
                IgnoreUnknownColumns = true // Linea para evitar errores o algunos datos no desead0s
            };
            CsvContext cc = new CsvContext();
            //Este IEnumerable tiene cada modelo que fue llenado con los datos del CSV
            IEnumerable<ListaClase> datos = cc.Read<ListaClase>(path, inputFileDescription);
            List<ListaClase> lista = datos.ToList();

            ValidadorListaClase val = new ValidadorListaClase();
            bool error;
            int filaActual = 0;
            foreach (ListaClase f in lista)
            {
                ++filaActual;
                error = val.Validar(f,filaActual).Item1;
                if (!error)
                {
                    return Tuple.Create(false, val.Validar(f,filaActual).Item2);
                }
            }

            //Se valida cada fila de CSV
            foreach (ListaClase f in lista)
            {
                insertarListaClase(f);
            }
            return Tuple.Create(true, "");
        }

        private void insertarListaClase(ListaClase fila)
        {
            db.InsertarUnidadCSV(fila.CodigoUnidad, fila.NombreFacultad);
            db.InsertarCarreraCSV(fila.CodigoCarrera, fila.NombreCarrera);
            db.InsertarEnfasisCSV(fila.CodigoCarreraEnfasis, fila.CodigoEnfasis, fila.NombreEnfasis);
            db.InsertarInscrita_En(fila.CodigoUnidadCarrera, fila.CodigoCarreraUnidad);
        }

        public Tuple<bool, string> cargarGuia(string path)
        {
            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',', //Indica qué es lo que separa cada valor en el archivo
                FirstLineHasColumnNames = true, //La primera fila corresponde a los títulos de los campos, no a un campo específico
                IgnoreUnknownColumns = true // Linea para evitar errores o algunos datos no desead0s
            };
            CsvContext cc = new CsvContext();
            //Este IEnumerable tiene cada modelo que fue llenado con los datos del CSV
            IEnumerable<GuiaHorario> datos = cc.Read<GuiaHorario>(path, inputFileDescription);
            List<GuiaHorario> lista = datos.ToList();


            ValidadorGuia val = new ValidadorGuia();
            bool error;
            int filaActual=0;
            foreach (GuiaHorario f in lista)
            {
                ++filaActual;
                error = val.Validar(f,filaActual).Item1;
                if (!error)
                {
                    return Tuple.Create(false, val.Validar(f, filaActual).Item2);
                }
            }

            //Se valida cada fila de CSV
            foreach (GuiaHorario f in lista)
            {
                insertarGuia(f);
            }
            return Tuple.Create(true, "");
        }

        private void insertarGuia(GuiaHorario fila)
        {
            db.InsertarCursoCSV(fila.SiglaCurso, fila.NombreCurso);
            db.InsertarGrupoCSV(fila.SiglaCursoGrupo, Convert.ToByte(fila.NumeroGrupo), Convert.ToByte(fila.Semestre), Convert.ToInt32(fila.Anno));
            //db.InsertarImparte(fila.CorreoProfesorImparte, fila.SiglaCursoImparte, Convert.ToByte(fila.NumeroGrupoImparte), Convert.ToByte(fila.SemestreGrupoImparte), Convert.ToInt32(fila.AnnoGrupoImparte));
            db.InsertarMatriculado_en(fila.CorreoMatricula, fila.SiglaCursoMatricula, Convert.ToByte(fila.NumeroGrupoMatricula), Convert.ToByte(fila.SemestreMatricula), Convert.ToInt32(fila.AnnoMatricula));
            db.InsertarPertenece_a(fila.CodigoCarreraCurso, fila.CodigoEnfasisCurso, fila.SiglaCursoCarrera);
        }

        public Tuple<bool, string> cargarListaEstudiante(string path)
        {
            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',', //Indica qué es lo que separa cada valor en el archivo
                FirstLineHasColumnNames = true, //La primera fila corresponde a los títulos de los campos, no a un campo específico
                IgnoreUnknownColumns = true // Linea para evitar errores o algunos datos no desead0s
            };
            CsvContext cc = new CsvContext();
            //Este IEnumerable tiene cada modelo que fue llenado con los datos del CSV
            IEnumerable<ListaEstudiante> datos = cc.Read<ListaEstudiante>(path, inputFileDescription);
            List<ListaEstudiante> lista = datos.ToList();

            ValidadorListaDeEstudiantes val = new ValidadorListaDeEstudiantes();
            bool error;
            int filaActual = 0;
            foreach (ListaEstudiante f in lista)
            {
                ++filaActual;
                error = val.Validar(f,filaActual).Item1;
                if (!error)
                {
                    return Tuple.Create(false, val.Validar(f,filaActual).Item2);
                }
            }

            //Se valida cada fila de CSV
            foreach (ListaEstudiante f in lista)
            {
                insertarListaEstudiante(f);
            }
            return Tuple.Create(true, "");
        }

        private void insertarListaEstudiante(ListaEstudiante fila)
        {
            db.InsertarPersonaCSV(fila.CorreoPersona, fila.IdPersona, fila.NombrePersona, fila.ApellidoPersona, fila.TipoIdPersona, Convert.ToBoolean(fila.Borrado));
            db.InsertarEstudianteCSV(fila.CorreoEstudiante);
            db.InsertarEmpadronadoEn(fila.CorreoEstudianteEmpadronado, fila.CodigoCarreraEmpadronado, fila.CodigoEnfasisEmpadronado);
        }

        public Tuple<bool, string> cargarListaFuncionario(string path)
        {
            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',', //Indica qué es lo que separa cada valor en el archivo
                FirstLineHasColumnNames = true, //La primera fila corresponde a los títulos de los campos, no a un campo específico
                IgnoreUnknownColumns = true // Linea para evitar errores o algunos datos no desead0s
            };
            CsvContext cc = new CsvContext();
            //Este IEnumerable tiene cada modelo que fue llenado con los datos del CSV
            IEnumerable<ListaFuncionario> datos = cc.Read<ListaFuncionario>(path, inputFileDescription);
            List<ListaFuncionario> lista = datos.ToList();

            //Se valida cada fila de CSV
            ValidadorListaFuncionarios val = new ValidadorListaFuncionarios();
            bool error;
            int filaActual = 0;
            foreach (ListaFuncionario f in lista)
            {
                ++filaActual;
                error = val.Validar(f,filaActual).Item1;
                if (!error)
                {
                    return Tuple.Create(false, val.Validar(f,filaActual).Item2);
                }
            }

            foreach (ListaFuncionario f in lista)
            {
                insertarListaFuncionario(f);
            }
            return Tuple.Create(true, "");
        }

        private void insertarListaFuncionario(ListaFuncionario fila)
        {
            db.InsertarPersonaCSV(fila.CorreoPersona, fila.IdPersona, fila.NombrePersona, fila.ApellidoPersona, fila.TipoIdPersona, Convert.ToBoolean(fila.Borrado));
            db.InsertarFuncionarioCSV(fila.CorreoFuncionario); //TODO: Que pasa si no es profesor?
            db.InsertarProfesorCSV(fila.CorreoProfesor);
            db.InsertarTrabajaEn(fila.CorreoFuncionarioTrabaja, fila.CodigoUnidadTrabaja); 
        }

    }
}
