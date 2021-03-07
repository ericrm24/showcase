using AppIntegrador;
using System.Data.Entity;
using AppIntegrador.Controllers;
using AppIntegrador.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using EntityFramework.Testing;

namespace AppIntegrador.Tests.Controllers
{
    /* TAM 3.7 TaskId - 2 Prueba de unidad para cargar perfiles que tiene un usuario*/
    [TestClass]
    public class PermissionsControllerTest
    {
        [TestMethod]
        public void TestCargarPerfil()
        {
            Init("admin@mail.com");
            PermissionsController controller = new PermissionsController();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            JsonResult perfiles = controller.CargarPerfil();
            string test = "{\"data\":[\"Superusuario\"]}";
            string result = new JavaScriptSerializer().Serialize(perfiles.Data);

            Assert.AreEqual(test, result);
        }

        [TestMethod]
        public void TestCargarUsuariosPermisos()
        {
            Init("admin@mail.com");
            PermissionsController controller = new PermissionsController();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            JsonResult checkboxes = controller.CargarCheckboxes("8", CurrentUser.getUserProfile(), "0000000001", "0000000001");
            string result = new JavaScriptSerializer().Serialize(checkboxes.Data);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCargarCheckboxesNull()
        {
            PermissionsController controller = new PermissionsController();
            JsonResult checkboxes = controller.CargarCheckboxes(null, null, "0000000001", "0000000001");
            string result = new JavaScriptSerializer().Serialize(checkboxes.Data);
            string expected = "{\"persons\":\"\",\"permissions\":\"\"}";
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void TestEntrarCargarUsuariosSinPermiso()
        {
            Init("andres@mail.com");
            CurrentUser.setCurrentUser("andres@mail.com", "Estudiante", "0000000001", "0000000001");
            PermissionsController controller = new PermissionsController();

            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;
            System.Web.Routing.RouteValueDictionary dictionary = new System.Web.Routing.RouteValueDictionary();
            dictionary.Add("action", "Index");
            dictionary.Add("controller", "Home");
            RedirectToRouteResult expected = new RedirectToRouteResult(dictionary);
            Assert.AreEqual(controller.TempData["alertmessage"], "No tiene permisos para acceder a esta página.");
            Assert.AreEqual(result.RouteValues["action"], expected.RouteValues["action"]);
            Assert.AreEqual(result.RouteValues["controller"], expected.RouteValues["controller"]);
        }

        [TestMethod]
        public void TestIndexNotNull()
        {
            Init("admin@mail.com");
            PermissionsController controller = new PermissionsController();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            Assert.IsNotNull(controller.Index());
        }

        [TestMethod]
        public void TestSeleccionarPerfilNotNull()
        {
            Init("admin@mail.com");
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            PermissionsController controller = new PermissionsController();
            Assert.IsNotNull(controller.SeleccionarPerfil());
        }

        [TestMethod]
        public void TestGuardarSeleccion()
        {
            Init("andres@mail.com");
            PermissionsController controller = new PermissionsController();
            CurrentUser.setCurrentUser("andres@mail.com", "Estudiante", "0000000001", "0000000001");
            controller.GuardarSeleccion("Profesor", "0000000001", "0000000001");
            Assert.AreEqual(CurrentUser.getUserProfile(), "Profesor");
            Assert.AreEqual(CurrentUser.getUserMajorId(), "0000000001");
            Assert.AreEqual(CurrentUser.getUserEmphasisId(), "0000000001");
        }

        [TestMethod]
        public void TestCargarEnfasisDeCarrera()
        {
            Init("admin@mail.com");
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            PermissionsController controller = new PermissionsController();
            JsonResult json = controller.CargarEnfasisDeCarrera("0000000001");
            string result = new JavaScriptSerializer().Serialize(json.Data);
            string expected = "{\"data\":[\"0000000000,Tronco Común\",\"0000000001,Ingeniería de Software\",\"0000000002,Ciencias de la Computación\",\"0000000003,Tecnologías de la Información\"]}";
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void TestCargarDatosDefault()
        {
            string correo = "admin@mail.com";
            var persona = new Persona()
            {
                Correo = correo,
                Identificacion = "123456781",
                Apellido1 = "Admin",
                Nombre1 = "Admin",
                TipoIdentificacion = "Cédula"
            };

            var usuario = new Usuario()
            {
                Activo = true
            };

            var carreras = new List<Carrera>
            {
                new Carrera() { Codigo = "0000000001", Nombre = "Bachillerato en Computación" }
            };

            var enfasis = new List<Enfasis>
            {
                new Enfasis() { CodCarrera = "0000000001", Codigo = "0000000000", Nombre = "Tronco Común" },
                new Enfasis() { CodCarrera = "0000000001", Codigo = "0000000001", Nombre = "Ingeniería de Software" }
            };

            var perfiles = new List<Perfil>
            {
                new Perfil() { Nombre = "Administrador" },
                new Perfil() { Nombre = "Director" }
            };

            var database = new Mock<DataIntegradorEntities>();

            database.Setup(m => m.Persona.Find(correo)).Returns(persona);
            database.Setup(m => m.Usuario.Find(correo)).Returns(usuario);

            database.Setup(m => m.Carrera).Returns(new Mock<DbSet<Carrera>>().SetupData(carreras, o => {
                return carreras.Single(x => x.Codigo == (string)o.First());
            }).Object);

            database.Setup(m => m.Enfasis).Returns(new Mock<DbSet<Enfasis>>().SetupData(enfasis, o => {
                return enfasis.Single(x => x.CodCarrera == (string)o.First() && x.Codigo == (string)o.First());
            }).Object);

            database.Setup(m => m.Perfil).Returns(new Mock<DbSet<Perfil>>().SetupData(perfiles, o => {
                return perfiles.Single(x => x.Nombre == (string)o.First());
            }).Object);

            Init("admin@mail.com");
            CurrentUser.setCurrentUser("admin@mail.com", "Administrador", "0000000001", "0000000000");

            PermissionsController controller = new PermissionsController(database.Object);
            JsonResult json = controller.CargarDatosDefault();
            string result = new JavaScriptSerializer().Serialize(json.Data);
            string expected = "{\"defaultProfile\":\"Administrador\",\"defaultMajor\":\"0000000001,Bachillerato en Computación\",\"defaultEmphasis\":\"0000000001,Ingeniería de Software\"}";
            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void TestGuardarPermisosSinPermiso()
        {
            Init("andres@mail.com");
            CurrentUser.setCurrentUser("andres@mail.com", "Estudiante", "0000000001", "0000000001");
            var httpContext = new HttpContext(
                new HttpRequest("", "http://localhost:44334/Home/Login", ""),
                new HttpResponse(new StringWriter())
            );
            var tempData = new TempDataDictionary();
            PermissionsController controller = new PermissionsController()
            {
                TempData = tempData
            };
            PermissionsViewHolder model = new PermissionsViewHolder();
            RedirectToRouteResult result = controller.Index(model) as RedirectToRouteResult;
            System.Web.Routing.RouteValueDictionary dictionary = new System.Web.Routing.RouteValueDictionary();
            dictionary.Add("action", "../Home/Index");
            RedirectToRouteResult expected = new RedirectToRouteResult(dictionary);
            Assert.AreEqual(controller.TempData["alertmessage"], "No tiene permisos para acceder a esta página.");
            Assert.AreEqual(result.RouteValues["action"], expected.RouteValues["action"]);
        }

        [TestMethod]
        public void CargarEnfasisDeCarreraPorPerfil()
        {
            //Arrange
            Init("admin@mail.com");
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            //Act
            PermissionsController controller = new PermissionsController();
            JsonResult json = controller.CargarEnfasisDeCarreraPorPerfil("0000000001", "Superusuario");
            string result = new JavaScriptSerializer().Serialize(json.Data);
            string expected = "{\"data\":[\"0000000000,Tronco Común\",\"0000000001,Ingeniería de Software\",\"0000000002,Ciencias de la Computación\",\"0000000003,Tecnologías de la Información\"]}";

            //Assert
            Assert.AreEqual(expected, result);
        }

        public void Init(string username)
        {
            //No aseguramos que admin no haya quedado logeado por otros tests.
            CurrentUser.deleteCurrentUser(username);

            // We need to setup the Current HTTP Context as follows:            

            // Step 1: Setup the HTTP Request
            var httpRequest = new HttpRequest("", "http://localhost/", "");

            // Step 2: Setup the HTTP Response
            var httpResponse = new HttpResponse(new StringWriter());

            // Step 3: Setup the Http Context
            var httpContext = new HttpContext(httpRequest, httpResponse);
            var sessionContainer =
                new HttpSessionStateContainer(username,
                                               new SessionStateItemCollection(),
                                               new HttpStaticObjectsCollection(),
                                               10,
                                               true,
                                               HttpCookieMode.AutoDetect,
                                               SessionStateMode.InProc,
                                               false);
            httpContext.Items["AspSession"] =
                typeof(HttpSessionState)
                .GetConstructor(
                                    BindingFlags.NonPublic | BindingFlags.Instance,
                                    null,
                                    CallingConventions.Standard,
                                    new[] { typeof(HttpSessionStateContainer) },
                                    null)
                .Invoke(new object[] { sessionContainer });

            var fakeIdentity = new GenericIdentity(username);
            var principal = new GenericPrincipal(fakeIdentity, null);

            // Step 4: Assign the Context
            HttpContext.Current = httpContext;
            HttpContext.Current.User = principal;
        }

        [TestCleanup]
        public void Cleanup()
        {
            CurrentUser.deleteAllUsers();
        }
    }
}
