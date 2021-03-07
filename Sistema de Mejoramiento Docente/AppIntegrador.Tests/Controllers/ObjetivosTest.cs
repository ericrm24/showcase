using AppIntegrador;
using AppIntegrador.Controllers;
using AppIntegrador.Controllers.PlanesDeMejoraBI;
using AppIntegrador.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace AppIntegrador.Tests.Controllers
{
    /// <summary>
    /// Summary description for ObjetivosTest
    /// </summary>
    [TestClass]
    public class ObjetivosTest
    {
        public ObjetivosTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void AnadirObjetivosTest()
        {
            var planesDeMejoraUtilMock = new Mock<PlanesDeMejoraUtil>();
            planesDeMejoraUtilMock
            .Setup(m => m.getView(It.IsAny<PartialViewResult>(), It.IsAny<ControllerContext>()))
            .Returns("success");

            var oc = new ObjetivosController(null, planesDeMejoraUtilMock.Object);
            List<Objetivo> objetivos = new List<Objetivo>
            {
                new Objetivo()
            };


            var result = oc.AnadirObjetivos(objetivos);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ObtenerSeccionesTest()
        {
            //var dbMock = new Mock<DataIntegradorEntities>();
            var planesDeMejoraUtilMock = new Mock<PlanesDeMejoraUtil>();
            List<string> codigosFormularios = new List<string> { "23", "34" };
            var formulario_tiene_seccion = new List<Formulario_tiene_seccion>
            {
                new Formulario_tiene_seccion{FCodigo = "23", SCodigo = "S1"},
                new Formulario_tiene_seccion{FCodigo = "34", SCodigo = "S2"}
            };

            //dbMock
            //.Setup(m => m.Seccion_tiene_pregunta.Where());
            planesDeMejoraUtilMock
                .Setup(m => m.getView(It.IsAny<PartialViewResult>(), It.IsAny<ControllerContext>()))
                .Returns("success");

            var objetivosController = new ObjetivosController(null, planesDeMejoraUtilMock.Object);

            JsonResult result = (JsonResult)objetivosController.ObtenerSecciones(codigosFormularios);

            Assert.IsNotNull(result.Data);

            objetivosController.Dispose();
        }

        [TestMethod]
        public void TablaSeccionesAsociadasTest()
        {
            //var dbMock = new Mock<DataIntegradorEntities>();
            var planesDeMejoraUtilMock = new Mock<PlanesDeMejoraUtil>();
            //List<string> codigosSecciones = new List<string> { "23", "34" };
            //IEnumerable<string> codigosPreguntas = new List<string> { "53", "42" };

            //dbMock
            //.Setup(m => m.ObtenerPreguntasDeAccionDeMejora(10, "prueba", "desc prueba"))
            //.Returns((ObjectResult) codigosPreguntas);
            planesDeMejoraUtilMock
            .Setup(m => m.getView(It.IsAny<PartialViewResult>(), It.IsAny<ControllerContext>()))
            .Returns("success");

            var oc = new ObjetivosController(null, planesDeMejoraUtilMock.Object);


            var result = oc.TablaSeccionesAsociadas(10, "prueba");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void listaDeObjetivosTest()
        {
            var oc = new ObjetivosController();
            var result = oc.listaDeObjetivos(1);

            Assert.IsNotNull(result);

        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


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
