using AppIntegrador.Models;
using AppIntegrador.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using AppIntegrador.Models.Metadata;
using System.Threading.Tasks;

namespace AppIntegrador.Controllers
{
    /*TAM-3.1, 3.2, 3.4, 3.5, 3.6, 3.7, 4.1: Controlador que implementa la funcionalidad de la página de administración de permisos y perfiles. */
    public class PermissionsController : Controller
    {
        private DataIntegradorEntities db;

        private readonly IPerm permissionManager;

        public PermissionsController()
        {
            db = new DataIntegradorEntities();
            permissionManager = new PermissionManager();
        }

        public PermissionsController(DataIntegradorEntities db)
        {
            this.db = db;
        }

        [HttpGet]
        public ActionResult Index()
        {
            /*Solo se puede acceder a este método si el usuario tiene un perfil con los permisos apropiados.*/
            if (!permissionManager.IsAuthorized(Permission.VER_PERMISOS_Y_PERFILES))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("Index", "Home");
            }
            PermissionsViewHolder model = new PermissionsViewHolder();

            return View(model);
        }

        /*TAM-3.3-1, 3.7-1.*/
        /*Método que lanza a la página de selección de perfil. Todos los usuarios tienen acceso a esto.*/
        [HttpGet]
        public ActionResult SeleccionarPerfil()
        {
            return View(new ConfigViewHolder());
        }
        /*Fin TAM-3.3.1, 3.7.1*/


        /// <summary>Este método guarda la selección de perfil, carrera y énfasis que el usuario haya seleccionado.</summary>
        /// <param name="ListaPerfiles">Código del perfil que el usuario seleccionó.</param>
        /// <param name="ListaCarreras">Código de la carrera que el usuario seleccionó.</param>
        /// <param name="ListaEnfasis">Código del énfasis que el usuario seleccionó.</param>
        /// <returns>ActionResult que redirige a la página Home/Index.</returns>
        [HttpPost]
        public ActionResult GuardarSeleccion(string ListaPerfiles, string ListaCarreras, string ListaEnfasis)
        {
            CurrentUser.setUserProfile(ListaPerfiles);
            CurrentUser.setUserMajor(ListaCarreras);
            CurrentUser.setUserEmphasis(ListaEnfasis);

            TempData["sweetalertmessage"] = "Cambios guardados";
            return RedirectToAction("Index", "Home");
        }
        /*Fin TAM-3.7.1*/

        /*TAM 3.4-1*/
        /// <summary>Este método guarda la combinación de selección de perfil, carrera, énfasis, personas y permisos.</summary>
        /// <param name="model">Objeto que contiene los datos de la selección.</param>
        /// <param name="isUser">Booleano para indicar si se están guardando datos de asignación de perfiles a personas
        /// o de permisos a perfiles.</param>
        /// <returns>ActionResult que recarga los datos de la página de administración de perfiles.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(PermissionsViewHolder model)
        {
            if (!permissionManager.IsAuthorized(Permission.EDITAR_PERMISOS_Y_PERFILES))
            {
                TempData["alertmessage"] = "No tiene permisos para acceder a esta página.";
                return RedirectToAction("../Home/Index");
            }

            int codPerfil = model.PerfilesSeleccionados;
            string perfil = model.Perfiles[codPerfil].NombrePerfil;
            string codCarrera = model.CarrerasSeleccionadas;
            string codEnfasis = model.EnfasisSeleccionados;
            List<Persona> Personas = model.Personas;
            List<Permiso> Permisos = model.Permisos;

            if ((perfil == null) || (codCarrera == null) || (codEnfasis == null))
            {
                TempData["alertmessage"] = "Algo salió mal. Intente de nuevo.";
                return RedirectToAction("Index", "Home");
            }

            if (permissionManager.IsAuthorized(Permission.ASIGNAR_PERFILES_USUARIOS))
            {
                //Guarda las asignaciones de un perfil en una carrera y un enfasis a los usuarios
                for (int i = 0; i < Personas.Count; ++i)
                {
                    db.AgregarUsuarioPerfil(Personas[i].Correo, perfil, codCarrera, codEnfasis, Personas[i].HasProfileInEmph);
                }
            }

            if(permissionManager.IsAuthorized(Permission.ASIGNAR_PERMISOS_PERFILES))
            {
                //Guarda las asignaciones de permisos a un perfil en una carrera y un enfasis
                for (int i = 0; i < Permisos.Count; ++i)
                {
                    db.AgregarPerfilPermiso(perfil, Permisos[i].Id, codCarrera, codEnfasis, Permisos[i].ActiveInProfileEmph);
                }
            }
            ViewBag.resultmessage = "Los cambios han sido guardados";
            return View(model);
        }
        /* Fin TAM 3.4-1.*/

        /*TAM-3.2*/
        /// <summary>Este método Se llama cuando se selecciona un énfasis en la página, para cargar los checkboxes según la configuración seleccionada.</summary>
        /// <param name="profileCode">Código del perfil seleccionado.</param>
        /// <param name="profileName">Nombre del perfil seleccionado.</param>
        /// <param name="majorCode">Código de la carrera seleccionada.</param>
        /// <param name="emphCode">Código del énfasis de la carrera seleccionado.</param>
        /// <returns>Objeto JsonResult que contiene los datos de los checkboxes en este formato, para ser renderizados por la vista.</returns>
        [HttpPost]
        public JsonResult CargarCheckboxes(string profileCode, string profileName, string majorCode, string emphCode)
        {
            if ((profileCode == null) || (profileName == null) || (majorCode == null) || (emphCode == null)) {
                TempData["alertmessage"] = "Algo salió mal. Intente de nuevo.";
                return Json(new { persons = "", permissions = "" });
            }
            PermissionsViewHolder model = new PermissionsViewHolder();
            // Obtener nombre de Perfil y Énfasis

            // Actualizar los checkboxes con la selección de énfasis.

            ObjectParameter tienePerfil = new ObjectParameter("tienePerfil", typeof(bool));
            ObjectParameter tieneActivo = new ObjectParameter("tieneActivo", typeof(bool));
            // Para revisar si el usuario tiene todos esos perfiles
            int total;
            int correct;

            // Revisa los checks de las personas
            foreach (Persona persona in model.Personas)
            {
                total = 0;
                correct = 0;

                ++total;
                // Se asume una sola carrera y un solo énfasis
                db.TienePerfilEnElEnfasis(persona.Correo, profileName, majorCode, emphCode, tienePerfil);

                if ((bool)tienePerfil.Value)
                {
                    // Si tiene el perfil asignado, aumente contador
                    ++correct;
                }

                // Tiene al menos un perfil
                if (correct > 0)
                {
                    // Tiene todos los perfiles
                    if (total == correct)
                    {
                        persona.HasProfileInEmph = true;
                    }
                }
            }

            // Revisa los checks de los permisos
            foreach (Permiso permiso in model.Permisos)
            {
                total = 0;
                correct = 0;

                ++total;
                // Se asume una sola carrera y un solo énfasis
                db.TienePermisoActivoEnEnfasis(permiso.Id, profileName, majorCode, emphCode, tieneActivo);

                if ((bool)tieneActivo.Value)
                {
                    // Si está activado en el perfil, aumente contador
                    ++correct;
                }


                // Activo en al menos un perfil
                if (correct > 0)
                {
                    // Activo en todos los perfiles
                    if (total == correct)
                    {
                        permiso.ActiveInProfileEmph = true;
                    }
                }
            }
            return Json(new { persons = PermissionManagerViewBuilder.ListPersonProfiles(model.Personas), permissions = PermissionManagerViewBuilder.ListProfilePermissions(model.Permisos) });
        }
        /*Fin TAM-3.2*/

        /*TAM-6.3-1.*/
        /// <summary>Este método envía a la vista los énfasis de la carrera que se haya seleccionado en formato JSON.</summary>
        /// <param name="value">Código de la carrera seleccionada.</param>
        /// <returns>Objeto JsonResult que contiene los datos de los énfasis de la carrera este formato, para ser renderizados por la vista.</returns>
        [HttpGet]
        public JsonResult CargarEnfasisDeCarrera(string value)
        {
            List<string> enfasis = new List<string>();
            using (var context = new DataIntegradorEntities())
            {
                /*Utiliza una función almacenada para obtener los énfasis que tiene una determinada carrera.*/
                var listaEnfasis = from Carrera in db.EnfasisXCarrera(value)
                                   select Carrera;
                foreach (var codigoEnfasis in listaEnfasis)
                {
                    string nombreEnfasis = db.Enfasis.Find(value, codigoEnfasis.codEnfasis).Nombre;
                    /*TAM-11.1: En la página de administración de perfiles y permisos solo se muestran las carreras y énfasis en los que el usuario tiene potestad en los dropdowns.*/
                    if (permissionManager.IsAllowed(CurrentUser.getUsername(), CurrentUser.getUserProfile(), value, codigoEnfasis.codEnfasis, Permission.ASIGNAR_PERFILES_USUARIOS) ||
                        permissionManager.IsAllowed(CurrentUser.getUsername(), CurrentUser.getUserProfile(), value, codigoEnfasis.codEnfasis, Permission.ASIGNAR_PERMISOS_PERFILES))
                        enfasis.Add(codigoEnfasis.codEnfasis + "," + nombreEnfasis);
                }

            }
            return Json(new { data = enfasis }, JsonRequestBehavior.AllowGet);
        }
        /*Fin TAM-6.3-1*/

        /*TAM-3.7-1.*/
        /// <summary>Este método envía a la vista los énfasis asociados a la carrera y perfil que se hayan seleccionado, en formato JSON.</summary>
        /// <param name="value">Código de la carrera seleccionada.</param>
        /// <param name="profile">Nombre del perfil seleccionado.</param>
        /// <returns>Objeto JsonResult que contiene los datos de los énfasis de la carrera por perfil este formato, para ser renderizados por la vista.</returns>
        [HttpGet]
        public JsonResult CargarEnfasisDeCarreraPorPerfil(string value, string profile)
        {
            List<string> enfasis = new List<string>();
            using (var context = new DataIntegradorEntities())
            {
                /*Utiliza una función almacenada para obtener los énfasis de la carrera asociada a un usuario, según el perfil seleccionado.*/
                var listaEnfasis = from Enfasis in db.EnfasisXCarreraXPerfil(CurrentUser.getUsername(), value, profile)
                                   select Enfasis;
                foreach (var codigoEnfasis in listaEnfasis)
                {
                    string nombreEnfasis = db.Enfasis.Find(value, codigoEnfasis.codEnfasis).Nombre;
                    enfasis.Add(codigoEnfasis.codEnfasis + "," + nombreEnfasis);
                }

            }
            return Json(new { data = enfasis }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>Este método envía a la vista las carreras que tiene asociadas un usuario según el perfil que se haya seleccionado, en formato JSON.</summary>
        /// <param name="perfilSeleccionado">Nombre del perfil seleccionado.</param>
        /// <returns>Objeto JsonResult que contiene los datos de las carreras según el perfil en este formato, para ser renderizados por la vista.</returns>
        [HttpGet]
        public JsonResult CargarCarreras(string perfilSeleccionado)
        {
            List<string> carreras = new List<string>();
            using (var context = new DataIntegradorEntities())
            {
                /*Utiliza una función almacenada para obtener las carreras que tiene asociadas un usuario según su perfil.*/
                var listaCarreras = from Resultado in db.CarrerasXPerfilXUsuario(CurrentUser.getUsername(), perfilSeleccionado)
                                    select Resultado;
                foreach (var carrera in listaCarreras)
                {
                    carreras.Add(carrera.codCarrera + "," + carrera.nombreCarrera);
                }
            }
            return Json(new { data = carreras }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>Este método envía a la vista los perfiles que tiene un usuario, en formato JSON.</summary>
        /// <returns>Objeto JsonResult que contiene los datos de los perfiles del usuario este formato, para ser renderizados por la vista.</returns>
        [HttpGet]
        public JsonResult CargarPerfil()
        {
            string username = CurrentUser.getUsername();
            List<string> perfiles = new List<string>();
            using (var context = new DataIntegradorEntities())
            {
                /*Utiliza una función almacenada para obtener los perfiles asociados a un usuario.*/
                var listaPerfiles = from Perfil in db.PerfilesXUsuario(username)
                                    select Perfil;
                foreach (var nombrePerfil in listaPerfiles)
                    perfiles.Add(nombrePerfil.NombrePefil);
            }
            return Json(new { data = perfiles }, JsonRequestBehavior.AllowGet);
        }
        /*Fin TAM-3.7-1*/

        /// <summary>Este método envía a la vista los datos que el usuario tiene seleccionados por defecto antes de configurarlos.</summary>
        /// <returns>Objeto JsonResult que contiene los datos por defecto del usuario, para ser renderizados por la vista.</returns>
        [HttpGet]
        public JsonResult CargarDatosDefault()
        {
            string profile = CurrentUser.getUserProfile();
            Carrera carrera = db.Carrera.Find(CurrentUser.getUserMajorId());
            Enfasis enfasis = db.Enfasis.Find(CurrentUser.getUserMajorId(), CurrentUser.getUserEmphasisId());
            string major = carrera.Codigo + "," + carrera.Nombre;
            string emphasis = enfasis.Codigo + "," + enfasis.Nombre;
            return Json(new { defaultProfile = profile, defaultMajor = major, defaultEmphasis = emphasis }, JsonRequestBehavior.AllowGet);
        }

    }
    
}