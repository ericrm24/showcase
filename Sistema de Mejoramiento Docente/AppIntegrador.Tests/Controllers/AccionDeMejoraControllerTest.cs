using AppIntegrador;
using AppIntegrador.Controllers;
using AppIntegrador.Controllers.PlanesDeMejoraBI;
using AppIntegrador.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.SessionState;

namespace AppIntegrador.Tests.Controllers
{
    /// <summary>
    /// Summary description for AccionDeMejora
    /// </summary>
    [TestClass]
    public class AccionDeMejoraControllerTest
    {
        public AccionDeMejoraControllerTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestMethod]
        public void ObtenerPreguntasTest()
        {
            //var dbMock = new Mock<DataIntegradorEntities>();
            var planesDeMejoraUtilMock = new Mock<PlanesDeMejoraUtil>();
            List<string> codigosSecciones = new List<string> { "23", "34" };
            var seccion_Tiene_Preguntas = new List<Seccion_tiene_pregunta>
            {
                new Seccion_tiene_pregunta{SCodigo = "23", PCodigo = "Q1"},
                new Seccion_tiene_pregunta{SCodigo = "34", PCodigo = "Q2"}
            };

            //dbMock
                //.Setup(m => m.Seccion_tiene_pregunta.Where());
            planesDeMejoraUtilMock
                .Setup(m => m.getView(It.IsAny<PartialViewResult>(), It.IsAny<ControllerContext>()))
                .Returns("success");

            var accionDeMejoraController = new AccionDeMejoraController(null, planesDeMejoraUtilMock.Object);

            JsonResult result = (JsonResult) accionDeMejoraController.ObtenerPreguntas(codigosSecciones);

            Assert.IsNotNull(result.Data);

            accionDeMejoraController.Dispose();
        }

        [TestMethod]
        public void AccionesDeObjetivoTest()
        {
            var accionDeMejoraController = new AccionDeMejoraController();

            PartialViewResult result = (PartialViewResult) accionDeMejoraController.AccionesDeObjetivo(0, "prueba", false);

            Assert.IsNotNull(result);

            accionDeMejoraController.Dispose();
        }

        [TestMethod]
        public void TablaPreguntasAsociadasTest()
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

            var admc = new AccionDeMejoraController(null, planesDeMejoraUtilMock.Object);


            var result = admc.TablaPreguntasAsociadas(10, "prueba", "desc prueba");

            Assert.IsNotNull(result);


        }

        private TestContext testContextInstance;

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
