using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AppIntegrador;
using AppIntegrador.Controllers;
using AppIntegrador.Models;
using Moq;
using System.Web.Helpers;
using System.Reflection;
using System.Web;
using System.Security.Principal;
using System.Web.SessionState;
using System.IO;

namespace AppIntegrador.Tests.Controllers
{
    // Clase de prueba asociada a la historia
    // BKS2: Yo como administrativo quiero crer una sección de preguntas para estrucutrar un formulario
    [TestClass]
    public class SeccionControllerTest
    {
        [TestMethod]
        // Prueba de que la vista no sea nula
        public void TestIndexNotNull()
        {
            // Arrange
            SeccionController seccionController = new SeccionController();

            // Act
            ViewResult result = seccionController.Index("", "", "") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        // Prueba de que la vista no sea nula
        public void TestCreateNotNull()
        {
            // Arrange
            SeccionController seccionController = new SeccionController();

            // Act
            ViewResult result = seccionController.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        //RIP CF5
        // Prueba de que la vista no sea nula
        public void TestCreateIndexFiltroCodigoNotNull()
        {
            // Arrange
            SeccionController seccionController = new SeccionController();

            // Act
            ViewResult result = seccionController.Index("0000001", "", "") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        //RIP CF5
        // Prueba de que la vista no sea nula
        public void TestCreateIndexFiltroNotNull()
        {
            // Arrange
            SeccionController seccionController = new SeccionController();

            // Act
            ViewResult result = seccionController.Index(null, null, null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        //RIP CF5
        // Prueba de que la vista no sea nula
        [TestMethod]
        public void TestIndexSiNo()
        {
            // Arrange
            SeccionController seccionController = new SeccionController();

            // Se prueban las de si/no/nr
            ViewResult result = seccionController.Index("", "", "S") as ViewResult;
            Assert.IsNotNull(result);

        }

        public void TestIndexEscalar()
        {
            // Arrange
            SeccionController seccionController = new SeccionController();
            // Se prueban las escalares
            var result = seccionController.Index("", "", "E") as ViewResult;
            Assert.IsNotNull(result);
        }

        public void TestIndexRespuestaLibre()
        {
            // Arrange
            SeccionController seccionController = new SeccionController();
            // Se prueban las de respuesta libre
            var result = seccionController.Index("", "", "L") as ViewResult;
            Assert.IsNotNull(result);
        }
        public void TestIndexSeleccionMultiple()
        {
            // Arrange
            SeccionController seccionController = new SeccionController();
            // Se prueban las de seleccion multiple
            var result = seccionController.Index("", "", "M") as ViewResult;
            Assert.IsNotNull(result);
        }
        public void TestIndexSeleccionUnica()
        {
            // Arrange
            SeccionController seccionController = new SeccionController();

            // Se prueba las de seleccion unica
            var result = seccionController.Index("", "", "U") as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        //RIP CF5
        // Prueba de que la vista no sea nula
        public void TestCreateIndexFiltroEnunciadoNotNull()
        {
            // Arrange
            SeccionController seccionController = new SeccionController();

            // Act
            ViewResult result = seccionController.Index("", "Info", "") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCreateFilterNullNotNull()
        {
            SeccionController seccionController = new SeccionController();

            // Null
            ViewResult result = seccionController.Create(null, null, null, null) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        // RIP CF5
        public void TestCreateFilterCodigoNotNull()
        {
            SeccionController seccionController = new SeccionController();

            ViewResult result = seccionController.Create("00000001", "", "", "") as ViewResult;
            Assert.IsNotNull(result);

        }

        [TestMethod]
        // RIP CF5
        public void TestCreateFilterEnunciadoNotNull()
        {
            SeccionController seccionController = new SeccionController();

            ViewResult result = seccionController.Create(null, "Info", null, null) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCreateFilterTiposNotNull()
        {
            SeccionController seccionController = new SeccionController();

            ViewResult result = seccionController.Create("", "", "U", "") as ViewResult;
            Assert.IsNotNull(result);

            result = seccionController.Create("", "", "M", "") as ViewResult;
            Assert.IsNotNull(result);

            result = seccionController.Create("", "", "L", "") as ViewResult;
            Assert.IsNotNull(result);

            result = seccionController.Create("", "", "S", "") as ViewResult;
            Assert.IsNotNull(result);

            result = seccionController.Create("", "", "E", "") as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestAgregarPreguntas()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            SeccionController seccionController = new SeccionController(mockDb.Object);

            string codSeccion = "00000001";
            List<string> codPreguntas = new List<string> { "00000001", "00000002" };

            JsonResult result = seccionController.AgregarPreguntas(codSeccion, codPreguntas) as JsonResult;

            Assert.AreEqual("{ insertadoExitoso = True }", result.Data.ToString());
        }

        [TestMethod]
        public void ProbarVistaPreviaSeccExiste()
        {
            SeccionController controller = new SeccionController();
            var result = controller.SeccionConPreguntas("00000001") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProbarVistaPreviaSeccNoExiste()
        {
            SeccionController controller = new SeccionController();
            var result = controller.SeccionConPreguntas("NOEXISTE") as HttpStatusCodeResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProbarVistaPreviaNula()
        {
            SeccionController controller = new SeccionController();
            var result = controller.SeccionConPreguntas(null) as ViewResult;

            Assert.IsNull(result);
        }
        [TestMethod]
        public void ProbarActualizarBancoSeccionesNullParams()
        {
            SeccionController controller = new SeccionController();
            var result = controller.ActualizarBancoSecciones(null, null, null) as PartialViewResult;

            Assert.AreEqual("Ninguno", result.ViewBag.filtro);
        }

        [TestMethod]
        public void ProbarActualizarBancoSeccionesFiltroCodigo()
        {
            SeccionController controller = new SeccionController();
            var result = controller.ActualizarBancoSecciones("1", "00000001", "") as PartialViewResult;

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void ProbarActualizarBancoSeccionesFiltroNombre()
        {
            SeccionController controller = new SeccionController();
            var result = controller.ActualizarBancoSecciones("2", "", "Cuál") as PartialViewResult;

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void ProbarActualizarBancoSeccionesStringVacios()
        {
            SeccionController controller = new SeccionController();
            var result = controller.ActualizarBancoSecciones("", "", "") as PartialViewResult;

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void ProbarActualizarCrearSeccionNoNulo()
        {
            SeccionController controller = new SeccionController();
            var result = controller.ActualizarCrearSeccion() as PartialViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProbarObtenerSeccionesConPreguntasEditable()
        {
            SeccionController controller = new SeccionController();
            var result = controller.ObtenerSeccionesConPreguntasEditable("00000001") ;

            Assert.IsNotNull(result.Count);
        }

        [TestMethod]
        public void ProbarVistaPreviaParametroNulo()
        {
            SeccionController controller = new SeccionController();
            var result = controller.VistaPrevia(null) as HttpStatusCodeResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProbarVistaPreviaSeccionValida()
        {
            SeccionController controller = new SeccionController();
            var result = controller.VistaPrevia("00000001") as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProbarVistaPreviaSeccionInvalida()
        {
            SeccionController controller = new SeccionController();
            var result = controller.VistaPrevia("NOEXISTE") as HttpNotFoundResult;
            Assert.IsNotNull(result);
        }



        [TestInitialize]
        public void Init()
        {
            //No aseguramos que admin no haya quedado logeado por otros tests.
            CurrentUser.deleteCurrentUser("admin@mail.com");

            // We need to setup the Current HTTP Context as follows:            

            // Step 1: Setup the HTTP Request
            var httpRequest = new HttpRequest("", "http://localhost/", "");

            // Step 2: Setup the HTTP Response
            var httpResponse = new HttpResponse(new StringWriter());

            // Step 3: Setup the Http Context
            var httpContext = new HttpContext(httpRequest, httpResponse);
            var sessionContainer =
                new HttpSessionStateContainer("admin@mail.com",
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

            var fakeIdentity = new GenericIdentity("admin@mail.com");
            var principal = new GenericPrincipal(fakeIdentity, null);

            // Step 4: Assign the Context
            HttpContext.Current = httpContext;
            HttpContext.Current.User = principal;
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "00000001", "00000001");
        }

        [TestCleanup]
        public void Cleanup()
        {
            //Nos aseguramos que admin quede deslogeado despues de cada test.
            CurrentUser.deleteCurrentUser("admin@mail.com");
        }
    }
}
