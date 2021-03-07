using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppIntegrador.Controllers;
using System.Web.Mvc;
using AppIntegrador.Models;
using Moq;
using System.Web.SessionState;
using System.Web;
using System.Security.Principal;
using System.Reflection;
using System.IO;

namespace AppIntegrador.Tests.Controllers
{
    /// <summary>
    /// Summary description for PreguntasTest
    /// </summary>
    [TestClass]
    public class PreguntasTest
    {
        public PreguntasTest()
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

        // Historia RIP-CBX
        [TestMethod]
        public void PreguntasGeneralNoNulo()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.Create() as ViewResult;

            Assert.IsNotNull(result);
        }



        // Historia RIP-CBX
        [TestMethod]
        public void RespuestaLibreNulo()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.RespuestaLibre() as ViewResult;

            Assert.IsNotNull(result);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void SeleccionUnicaNoNula()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.PreguntaConOpciones() as ViewResult;

            Assert.IsNotNull(result);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void OpcionesDeSeleccionNoNula()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.OpcionesDeSeleccion(1, 'U') as ViewResult;

            Assert.IsNotNull(result);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void EstudianteOpcionesDeSeleccionNula()
        {
            PreguntasController preguntas = new PreguntasController();
            CurrentUser.clearSession();

            CurrentUser.setCurrentUser("paco@mail.com", "Estudiante", "0000000001", "0000000001");
            ViewResult result = preguntas.OpcionesDeSeleccion(-1, 'U') as ViewResult;

            Assert.IsNull(result);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void OpcionesDeSeleccionNula()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.OpcionesDeSeleccion(-1, 'U') as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ActualizarPreguntasNula()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.ActualizarBancoPreguntas(null, null, null) as ViewResult;

            Assert.IsNull(result);
        }



        [TestMethod]
        public void ActualizarPreguntasNoNulo()
        {
            PreguntasController preguntas = new PreguntasController();

            PartialViewResult result = preguntas.ActualizarBancoPreguntas("00000001", "00000001", "00000001") as PartialViewResult;

            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void CrearBaseNoNulo()
        {
            PreguntasController preguntas = new PreguntasController();

            PartialViewResult result = preguntas.CreateBase() as PartialViewResult;

            Assert.IsNotNull(result);
        }


        // Historia RIP-CBX
        [TestMethod]
        public void EstilosNoNula()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.Estilos() as ViewResult;

            Assert.IsNotNull(result);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void GuardarRespuestaLibreNula()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.GuardarRespuestaLibre(null) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GuardarPreguntaLibre()
        {
            string codPregunta = "TEST´RIP";
            string enuncPregunta = "TEST";
            Pregunta pregunta = new Pregunta
            {
                Codigo = codPregunta,
                Tipo = "L",
                Enunciado = enuncPregunta
            };

            PreguntasController preguntas = new PreguntasController();

            var result = preguntas.GuardarRespuestaLibre(pregunta);

            preguntas.Dispose();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GuardarPreguntaLibreError()
        {
            string codPregunta = "";
            string enuncPregunta = "TEST";
            Pregunta pregunta = new Pregunta
            {
                Codigo = codPregunta,
                Tipo = "S",
                Enunciado = enuncPregunta
            };

            PreguntasController preguntas = new PreguntasController();

            var result = preguntas.GuardarRespuestaLibre(pregunta);

            preguntas.Dispose();
            Assert.IsNotNull(result);
        }


        // Historia RIP-CBX
        [TestMethod]
        public void CreatePostNoNula()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.Create() as ViewResult;

            Assert.IsNotNull(result);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void EstudianteCreaPregunta()
        {
            PreguntasController preguntas = new PreguntasController();
            preguntas.ModelState.Clear();
            CurrentUser.clearSession();
            CurrentUser.setCurrentUser("paco@mail.com", "Estudiante", "0000000001", "0000000001");


            Pregunta pregunta = new Pregunta
            {
                Codigo = "",
                Enunciado = ""
            };

            ViewResult result = preguntas.Create() as ViewResult;

            Assert.IsNull(result);
        }




        // Historia RIP-CBX
        [TestMethod]
        public void ProbarCrear()
        {
            var controller = new PreguntasController();
            var result = controller.Create() as ViewResult;

            Assert.AreEqual("Create", result.ViewName);
        }

        [TestMethod]
        public void ProbarCrearBase()
        {
            var controller = new PreguntasController();
            var result = controller.CreateBase() as PartialViewResult;

            Assert.AreEqual("Create", result.ViewName);
        }


        // Historia RIP-CBX
        [TestMethod]
        public void ProbarOpciones()
        {
            var controller = new PreguntasController();
            var result = controller.OpcionesDeSeleccion(7, 'U') as ViewResult;

            Assert.AreEqual("OpcionesDeSeleccion", result.ViewName);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void ProbarPreguntaConOpciones()
        {
            var controller = new PreguntasController();
            var result = controller.PreguntaConOpciones() as ViewResult;

            Assert.AreEqual("PreguntaConOpciones", result.ViewName);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void ProbarRespuestaLibre()
        {
            var controller = new PreguntasController();
            var result = controller.RespuestaLibre() as ViewResult;

            Assert.AreEqual("RespuestaLibre", result.ViewName);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void ProbarViewbagGeneral()
        {
            // Arrange
            var controller = new PreguntasController();
            // Act
            ViewResult result = controller.Create() as ViewResult;
            // Assert
            Assert.AreEqual("Crear pregunta", result.ViewBag.Message);
        }


        // Historia RIP-CBX
        [TestMethod]
        public void ProbarEstilos()
        {
            var controller = new PreguntasController();
            var result = controller.Estilos() as ViewResult;

            Assert.AreEqual("Estilos", result.ViewName);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void ProbarGuardarRespuestaLibre()
        {
            var controller = new PreguntasController();
            var result = controller.GuardarRespuestaLibre(null) as ViewResult;

            Assert.IsNull(result);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void ProbarPostNoNulaConPregunta()
        {
            PreguntasController preguntas = new PreguntasController();
            preguntas.ModelState.Clear();

            Pregunta pregunta = new Pregunta
            {
                Codigo = "",
                Enunciado = ""
            };

            ViewResult result = preguntas.Create() as ViewResult;

            Assert.AreEqual("Create", result.ViewName);
        }

        [TestMethod]
        public void ProbarVistaPreviaNula()
        {
            PreguntasController controller = new PreguntasController();
            var result = controller.VistaPrevia(null) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ProbarVistaPreviaPregExiste()
        {
            PreguntasController controller = new PreguntasController();
            var result = controller.VistaPrevia("00000001") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void EstudianteProbarVistaPreviaPregExiste()
        {
            PreguntasController controller = new PreguntasController();
            CurrentUser.clearSession();

            CurrentUser.setCurrentUser("paco@mail.com", "Estudiante", "0000000001", "0000000001");

            var result = controller.VistaPrevia("00000001") as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestEstudianteTodasLasPreguntas()
        {
            PreguntasController preguntas = new PreguntasController();
            preguntas.ModelState.Clear();

            CurrentUser.clearSession();

            CurrentUser.setCurrentUser("paco@mail.com", "Estudiante", "0000000001", "0000000001");
            ViewResult result = preguntas.TodasLasPreguntas("000001") as ViewResult;

            Assert.IsNull(result);
        }


        [TestMethod]
        public void TestTodasLasPreguntasValida()
        {
            PreguntasController preguntas = new PreguntasController();
            preguntas.ModelState.Clear();

            CurrentUser.clearSession();

            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "00000001", "00000001");
            ViewResult result = preguntas.TodasLasPreguntas("00000001") as ViewResult;
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void TestTodasLasPreguntasNoValida()
        {
            PreguntasController preguntas = new PreguntasController();
            preguntas.ModelState.Clear();

            CurrentUser.clearSession();

            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "00000001", "00000001");
            ViewResult result = preguntas.TodasLasPreguntas("0000001") as ViewResult;
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ProbarVistaPreviaPregNoExiste()
        {
            PreguntasController controller = new PreguntasController();
            var result = controller.VistaPrevia("NOEXISTE") as ViewResult;

            Assert.IsNull(result);
        }


        // ------- index
        [TestMethod]
        public void IndexNula()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.Index(null, null, null, null) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void IndexNoNulo()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.Index("00000001", "00000001", "00000001", "00000001") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void EstudianteIndex()
        {
            PreguntasController preguntas = new PreguntasController();

            CurrentUser.clearSession();
            CurrentUser.setCurrentUser("paco@mail.com", "Estudiante", "0000000001", "0000000001");

            ViewResult result = preguntas.Index("00000001", "00000001", "00000001", "00000001") as ViewResult;

            Assert.IsNull(result);
        }


        [TestMethod]
        public void TestIndexCodigo()
        {
            PreguntasController preguntas = new PreguntasController();

            CurrentUser.clearSession();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = preguntas.Index("", "00000001", "", "") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexEnunciado()
        {
            PreguntasController preguntas = new PreguntasController();

            CurrentUser.clearSession();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = preguntas.Index("", "", "Enunciado", "") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexTipoUnica()
        {
            PreguntasController preguntas = new PreguntasController();

            CurrentUser.clearSession();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = preguntas.Index("", "", "", "U") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexTipoMultiple()
        {
            PreguntasController preguntas = new PreguntasController();

            CurrentUser.clearSession();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = preguntas.Index("", "", "", "M") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexTipoLibre()
        {
            PreguntasController preguntas = new PreguntasController();

            CurrentUser.clearSession();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = preguntas.Index("", "", "", "L") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexTipoSiNo()
        {
            PreguntasController preguntas = new PreguntasController();

            CurrentUser.clearSession();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = preguntas.Index("", "", "", "S") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexTipoEscalar()
        {
            PreguntasController preguntas = new PreguntasController();

            CurrentUser.clearSession();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = preguntas.Index("", "", "", "E") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexTipoError()
        {
            PreguntasController preguntas = new PreguntasController();

            CurrentUser.clearSession();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = preguntas.Index("", "", "", "x") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexVacio()
        {
            PreguntasController preguntas = new PreguntasController();

            CurrentUser.clearSession();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = preguntas.Index("", "", "", "") as ViewResult;

            Assert.IsNotNull(result);
        }


        // ------------ ' si/no

        // Historia RIP-CBX
        [TestMethod]
        public void PreguntaSiNoNulo()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.PreguntaSiNo() as ViewResult;

            Assert.IsNotNull(result);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void ProbarPreguntaSiNo()
        {
            var controller = new PreguntasController();
            var result = controller.PreguntaSiNo() as ViewResult;

            Assert.AreEqual("PreguntaSiNo", result.ViewName);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void ProbarGuardarPreguntaSiNo()
        {
            var controller = new PreguntasController();
            var result = controller.GuardarPreguntaSiNo(null) as ViewResult;

            Assert.IsNull(result);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void GuardarPreguntaSiNoNula()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.GuardarPreguntaSiNo(null) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GuardarPreguntaSiNo()
        {
            string codPregunta = "TEST1234";
            string enuncPregunta = "TEST";
            Pregunta pregunta = new Pregunta
            {
                Codigo = codPregunta,
                Tipo = "S",
                Enunciado = enuncPregunta
            };

            Pregunta_con_opciones opciones = new Pregunta_con_opciones
            {
                TituloCampoObservacion = ""
            };

            pregunta.Pregunta_con_opciones = opciones;


            PreguntasController preguntas = new PreguntasController();

            var result = preguntas.GuardarPreguntaSiNo(pregunta);

            preguntas.Dispose();
            Assert.IsNotNull(result);
        }


        // ------------ ' escalar
        // Historia RIP-CBX
        [TestMethod]
        public void PreguntaEscalarNoNulo()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.PreguntaEscalar() as ViewResult;

            Assert.IsNotNull(result);
        }

        // Historia RIP-CBX
        [TestMethod]
        public void ProbarPreguntaEscalar()
        {
            var controller = new PreguntasController();
            var result = controller.PreguntaEscalar() as ViewResult;

            Assert.AreEqual("PreguntaEscalar", result.ViewName);
        }

        [TestMethod]
        public void ProbarGuardarPreguntaEscalar()
        {
            var controller = new PreguntasController();
            var result = controller.GuardarPreguntaEscalar(null, 0, 0) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GuardarPreguntaEscalar()
        {
            PreguntasController preguntas = new PreguntasController();

            ViewResult result = preguntas.GuardarPreguntaEscalar(null, 0, 0) as ViewResult;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GuardarPreguntaEscalarCrearMenor()
        {
            string codPregunta = "TESTRIP";
            string enuncPregunta = "TEST";
            Pregunta pregunta = new Pregunta
            {
                Codigo = codPregunta,
                Tipo = "E",
                Enunciado = enuncPregunta
            };

            Pregunta_con_opciones opciones = new Pregunta_con_opciones
            {
                TituloCampoObservacion = ""
            };

            pregunta.Pregunta_con_opciones = opciones;


            PreguntasController preguntas = new PreguntasController();

            var result = preguntas.GuardarPreguntaEscalar(pregunta, 10, -2);

            preguntas.Dispose();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GuardarPreguntaEscalarCrearMayor()
        {
            string codPregunta = "TESTRIP";
            string enuncPregunta = "TEST";
            Pregunta pregunta = new Pregunta
            {
                Codigo = codPregunta,
                Tipo = "E",
                Enunciado = enuncPregunta
            };

            Pregunta_con_opciones opciones = new Pregunta_con_opciones
            {
                TituloCampoObservacion = ""
            };

            pregunta.Pregunta_con_opciones = opciones;


            PreguntasController preguntas = new PreguntasController();

            var result = preguntas.GuardarPreguntaEscalar(pregunta, 1, 20);

            preguntas.Dispose();
            Assert.IsNotNull(result);
        }

        public void GuardarPreguntaEscalarCrearMayorError()
        {
            string codPregunta = "TESTRIP";
            string enuncPregunta = "TEST";
            Pregunta pregunta = new Pregunta
            {
                Codigo = codPregunta,
                Tipo = "L",
                Enunciado = enuncPregunta
            };

            Pregunta_con_opciones opciones = new Pregunta_con_opciones
            {
                TituloCampoObservacion = ""
            };

            pregunta.Pregunta_con_opciones = opciones;


            PreguntasController preguntas = new PreguntasController();

            var result = preguntas.GuardarPreguntaEscalar(pregunta, 1, 20);

            preguntas.Dispose();
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
