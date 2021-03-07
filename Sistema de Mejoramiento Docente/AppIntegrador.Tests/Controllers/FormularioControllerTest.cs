using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppIntegrador;
using AppIntegrador.Controllers;
using AppIntegrador.Models;
using Moq;
using System.Security.Principal;
using System.Web;
using System.Web.Routing;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.IO;
using System.Web.SessionState;
using System.Reflection;

namespace AppIntegrador.Tests.Controllers
{
    /// <summary>
    /// Summary description for FormularioControllerTest
    /// </summary>
    [TestClass]
    public class FormularioControllerTest
    {
        public FormularioControllerTest()
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

        [TestMethod]
        public void TestCreateNotNull()
        {
            FormulariosController formulario = new FormulariosController();
            ViewResult result = formulario.Create() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCreateView()
        {
            FormulariosController formulario = new FormulariosController();
            ViewResult result = formulario.Create() as ViewResult;
            Assert.AreEqual("Create", result.ViewName);
        }

        [TestMethod]
        public void TestIndexNotNull()
        {
            FormulariosController formulario = new FormulariosController();
            ViewResult result = formulario.Index(null, null, null) as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestIndexView()
        {
            FormulariosController formulario = new FormulariosController();
            ViewResult result = formulario.Index(null, null, null) as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void TestDetailsNull()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            FormulariosController controller = new FormulariosController(mockDb.Object);

            // Se prueba que el método no se caiga con parámetros nulos
            var result = controller.Details(null);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestDetails()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            FormulariosController controller = new FormulariosController(mockDb.Object);

            string codFormulario = "CI0128G2";
            string nombreFormulario = "Formulario de prueba";
            string codSeccion = "Secci01";

            // Se crea un formulario para el mock de la base de datos
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = nombreFormulario
            };

            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            Seccion seccion = new Seccion()
            {
                Codigo = codSeccion,
                Nombre = "Sección de prueba"
            };

            mockDb.Setup(m => m.Seccion.Find(codSeccion)).Returns(seccion);

            // Agregar la sección al formulario
            Formulario_tiene_seccion fts = new Formulario_tiene_seccion()
            {
                FCodigo = codFormulario,
                SCodigo = codSeccion
            };

            mockDb.Setup(m => m.Formulario_tiene_seccion.Find(codSeccion)).Returns(fts);

            var mock = new Mock<DbSet<Seccion>>();

            // Se prueba que el método no se caiga con un código de formulario válido
            var result = controller.Details(codSeccion);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestDeleteNull()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            FormulariosController controller = new FormulariosController(mockDb.Object);

            // Se prueba que el método no se caiga con parámetros nulos
            var result = controller.Delete(null);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestDelete()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            FormulariosController controller = new FormulariosController(mockDb.Object);

            string codFormulario = "CI0128G2";
            string nombreFormulario = "Formulario de prueba";
            string codSeccion = "Secci01";

            // Se crea un formulario para el mock de la base de datos
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = nombreFormulario
            };

            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            Seccion seccion = new Seccion()
            {
                Codigo = codSeccion,
                Nombre = "Sección de prueba"
            };

            mockDb.Setup(m => m.Seccion.Find(codSeccion)).Returns(seccion);

            // Agregar la sección al formulario
            Formulario_tiene_seccion fts = new Formulario_tiene_seccion()
            {
                FCodigo = codFormulario,
                SCodigo = codSeccion
            };

            mockDb.Setup(m => m.Formulario_tiene_seccion.Find(codSeccion)).Returns(fts);

            var mock = new Mock<DbSet<Seccion>>();

            // Se prueba que el método no se caiga con un código de formulario válido
            var result = controller.Delete(codSeccion);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestDeleteConfirmed()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            FormulariosController controller = new FormulariosController(mockDb.Object);

            string codFormulario = "CI0128G2";
            string nombreFormulario = "Formulario de prueba";
            string codSeccion = "Secci01";

            // Se crea un formulario para el mock de la base de datos
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = nombreFormulario
            };

            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            Seccion seccion = new Seccion()
            {
                Codigo = codSeccion,
                Nombre = "Sección de prueba"
            };

            mockDb.Setup(m => m.Seccion.Find(codSeccion)).Returns(seccion);

            // Agregar la sección al formulario
            Formulario_tiene_seccion fts = new Formulario_tiene_seccion()
            {
                FCodigo = codFormulario,
                SCodigo = codSeccion
            };

            mockDb.Setup(m => m.Formulario_tiene_seccion.Find(codSeccion)).Returns(fts);

            var mock = new Mock<DbSet<Seccion>>();

            // Se prueba que el método no se caiga con un código de formulario válido
            var result = controller.DeleteConfirmed(codSeccion);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestEditNull()
        {
            FormulariosController controller = new FormulariosController();

            // Se prueba que el método no se caiga con parámetros nulos
            ActionResult result = controller.Edit(null);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestEdit()
        {
            string codSeccion = "00000001";

            FormulariosController controller = new FormulariosController();

            // Se prueba que el método no se caiga con un código de formulario válido
            var result = controller.Edit(codSeccion);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestAgregarFormulario()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            FormulariosController controller = new FormulariosController(mockDb.Object);

            string codFormulario = "CI0128G2";
            string nombreFormulario = "Formulario de prueba";

            // Se crea un formulario para el mock de la base de datos
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = nombreFormulario
            };

            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            // Se prueba que el método no se caiga con un código de formulario válido
            var result = controller.AgregarFormulario(formulario);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestAgregarSeccion()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            FormulariosController controller = new FormulariosController(mockDb.Object);

            string codSeccion = "Secci01";

            Seccion seccion = new Seccion()
            {
                Codigo = codSeccion,
                Nombre = "Sección de prueba"
            };

            mockDb.Setup(m => m.Seccion.Find(codSeccion)).Returns(seccion);

            var mock = new Mock<DbSet<Seccion>>();

            // Se prueba que el método no se caiga con un código de formulario válido
            var result = controller.AgregarSeccion(seccion);

            Assert.IsNotNull(result);
        }

        
        [TestMethod]
        public void TestIndexFilters()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            FormulariosController controller = new FormulariosController(mockDb.Object);

            Formulario form = new Formulario()
            {
                Codigo = "CI0128IE",
                Nombre = "Sección de prueba"
            };

            Formulario form2 = new Formulario()
            {
                Codigo = "CI0122IE",
                Nombre = "Sección de p3ueba"
            };

            IQueryable<Formulario> formularios = new List<Formulario> { form, form2 }.AsQueryable();

            var mock = new Mock<DbSet<Formulario>>();

            mock.As<IQueryable<Formulario>>().Setup(m => m.Provider).Returns(formularios.Provider);
            mock.As<IQueryable<Formulario>>().Setup(m => m.Expression).Returns(formularios.Expression);
            mock.As<IQueryable<Formulario>>().Setup(m => m.ElementType).Returns(formularios.ElementType);
            mock.As<IQueryable<Formulario>>().Setup(m => m.GetEnumerator()).Returns(formularios.GetEnumerator());

            mockDb.Setup(x => x.Formulario).Returns(mock.Object);

            // Se prueba que el método no se caiga con parámetros nulos
            var result = controller.Index(null, null, null);
            Assert.IsNotNull(result);

            // Se prueba que el método no se caiga con un paramétro de código formulario real
            var result1 = controller.Index("CI0128", "", "");
            Assert.IsNotNull(result);

            // Se prueba que el método no se caiga con un parámetro de nombre real
            var result2 = controller.Index("", "Prueba", "");
            Assert.IsNotNull(result);

            // Se prueba que el método no se caiga con un parámetro de tipo de pregunta real
            var result3 = controller.Index("", "", "libre");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestAsociarSeccionesAFormulario()
        {
            // la variable para realizar el mock
            var mockDb = new Mock<DataIntegradorEntities>();

            string codFormulario = "CI0128G2";
            string nombreFormulario = "Formulario de prueba";
            string codSeccion = "Secci01";

            // Se crea un formulario para el mock de la base de datos
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = nombreFormulario
            };
            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            // Se crea una sección de prueba
            ObtenerSeccionesDeFormulario_Result seccion = new ObtenerSeccionesDeFormulario_Result
            {
                Codigo = codSeccion,
                Nombre = "Sección sobre algo",
                Orden = 0
            };

            List<string> seccionesAsociadas = new List<string>();
            seccionesAsociadas.Add(codSeccion);

            // Instancia del controller para accesar a los métodos que se probarán de FormulariosController
            FormulariosController controller = new FormulariosController(mockDb.Object);

            // Se llama el método del controller para ver si devuelve un resultado válido
            var result = controller.AsociarSeccionesAFormulario(codFormulario, nombreFormulario, seccionesAsociadas);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCreateFailed()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            string codFormulario = "CI0128G2";
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = "Formularios de prueba para CI0128"
            };

            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            FormulariosController controller = new FormulariosController(mockDb.Object);
            var result = controller.Create(formulario, 0);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCreateSuccesful()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            string codFormulario = "CI0128G2";
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = "Formularios de prueba para CI0128"
            };

            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            FormulariosController controller = new FormulariosController(mockDb.Object);
            var result = controller.Create(formulario, 1);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestAplicarFiltros()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            FormulariosController controller = new FormulariosController(mockDb.Object);

            Seccion seccion = new Seccion()
            {
                Codigo = "CI0128IE",
                Nombre = "Sección de prueba"
            };

            Seccion seccion2 = new Seccion()
            {
                Codigo = "CI0122IE",
                Nombre = "Sección de p3ueba"
            };

            IQueryable<Seccion> secciones = new List<Seccion> { seccion, seccion2 }.AsQueryable();

            var mock = new Mock<DbSet<Seccion>>();

            mock.As<IQueryable<Seccion>>().Setup(m => m.Provider).Returns(secciones.Provider);
            mock.As<IQueryable<Seccion>>().Setup(m => m.Expression).Returns(secciones.Expression);
            mock.As<IQueryable<Seccion>>().Setup(m => m.ElementType).Returns(secciones.ElementType);
            mock.As<IQueryable<Seccion>>().Setup(m => m.GetEnumerator()).Returns(secciones.GetEnumerator());

            mockDb.Setup(x => x.Seccion).Returns(mock.Object);

            // Se prueba que el método no se caiga con parámetros nulos
            var result = controller.AplicarFiltro(null,null,null);
            Assert.IsNotNull(result);

            // Se prueba que el método no se caiga con un paramétro de código formulario real
            var result1 = controller.AplicarFiltro("CI0128", "", "");
            Assert.IsNotNull(result);

            // Se prueba que el método no se caiga con un parámetro de nombre real
            var result2 = controller.AplicarFiltro("", "Prueba", "");
            Assert.IsNotNull(result);

            // Se prueba que el método no se caiga con un parámetro de tipo de pregunta real
            var result3 = controller.AplicarFiltro("", "", "libre");
            Assert.IsNotNull(result);
        }

        // RIP-ELF
        [TestMethod]
        public void TestObtenerSeccionesConPreguntas()
        {
            TestSetup testSetup = new TestSetup();
            var mockDb = new Mock<DataIntegradorEntities>();

            string codFormulario = "TESTPNUL";
            string codSeccion = "SECCPNUL";

            // Se crea el formulario de prueba
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = "Formulario de prueba con preguntas con opciones nulas"
            };

            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            ObtenerSeccionesDeFormulario_Result seccion = new ObtenerSeccionesDeFormulario_Result
            {
                Codigo = codSeccion,
                Nombre = "Sección de prueba con preguntas con opciones nulas",
                Orden = 0
            };

            SeccionController controller = new SeccionController(mockDb.Object);

            testSetup.SetupHttpContext(controller);

            var result = controller.ObtenerSeccionesConPreguntasEditable(codFormulario);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestBorrarSeccion()
        {
            TestSetup testSetup = new TestSetup();
            var mockDb = new Mock<DataIntegradorEntities>();

            string codFormulario = "TESTPSU";
            string codSeccion = "SECCPSU";
            string codPregunta = "PREGSU";

            // Se crea el formulario de prueba
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = "Formulario de prueba con preguntas de seleccion única"
            };

            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            ObtenerSeccionesDeFormulario_Result seccion = new ObtenerSeccionesDeFormulario_Result
            {
                Codigo = codSeccion,
                Nombre = "Sección de prueba",
                Orden = 0
            };

            var mockedObtenerSecciones = testSetup.SetupMockProcedure<ObtenerSeccionesDeFormulario_Result>
                (new List<ObtenerSeccionesDeFormulario_Result> { seccion });
            mockDb.Setup(x => x.ObtenerSeccionesDeFormulario(codFormulario)).Returns(mockedObtenerSecciones.Object);

            ObtenerPreguntasDeSeccion_Result pregunta = new ObtenerPreguntasDeSeccion_Result
            {
                Codigo = codPregunta,
                Enunciado = "¿Si no sé, es la _?",
                Tipo = "U",
                Orden = 0,
            };

            FormulariosController controller = new FormulariosController(mockDb.Object);

            testSetup.SetupHttpContext(controller);

            var result = controller.BorrarSeccion(codFormulario, codSeccion);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestEliminarSeccionNoNull()
        {
            TestSetup testSetup = new TestSetup();
            string codFormulario = "TESTPSU";
            string codSeccion = "SECCPSU";


            FormulariosController controller = new FormulariosController();

            testSetup.SetupHttpContext(controller);

            var result = controller.EliminarSeccion(codFormulario, codSeccion);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestEliminarPreguntaNoNull()
        {
            TestSetup testSetup = new TestSetup();
            string codSeccion = "SECCPSU";
            string codPregunta = "PREGSU";

            FormulariosController controller = new FormulariosController();

            testSetup.SetupHttpContext(controller);

            var result = controller.EliminarPregunta(codSeccion, codPregunta);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestEliminarSeccionNull()
        {
            TestSetup testSetup = new TestSetup();
            FormulariosController controller = new FormulariosController();

            testSetup.SetupHttpContext(controller);

            var result = controller.EliminarSeccion(null, null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestEliminarPreguntaNull()
        {
            TestSetup testSetup = new TestSetup();
            FormulariosController controller = new FormulariosController();

            testSetup.SetupHttpContext(controller);

            var result = controller.EliminarPregunta(null, null);

            Assert.IsNull(result);
        }

       
        [TestMethod]
        public void TestBorrarPregunta()
        {
            TestSetup testSetup = new TestSetup();
            var mockDb = new Mock<DataIntegradorEntities>();

            string codFormulario = "TESTPSU";
            string codSeccion = "SECCPSU";
            string codPregunta = "PREGSU";

            // Se crea el formulario de prueba
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = "Formulario de prueba con preguntas de seleccion única"
            };

            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            ObtenerSeccionesDeFormulario_Result seccion = new ObtenerSeccionesDeFormulario_Result
            {
                Codigo = codSeccion,
                Nombre = "Sección de prueba",
                Orden = 0
            };

            var mockedObtenerSecciones = testSetup.SetupMockProcedure<ObtenerSeccionesDeFormulario_Result>
                (new List<ObtenerSeccionesDeFormulario_Result> { seccion });
            mockDb.Setup(x => x.ObtenerSeccionesDeFormulario(codFormulario)).Returns(mockedObtenerSecciones.Object);

            ObtenerPreguntasDeSeccion_Result pregunta = new ObtenerPreguntasDeSeccion_Result
            {
                Codigo = codPregunta,
                Enunciado = "¿Si no sé, es la _?",
                Tipo = "U",
                Orden = 0,
            };

            FormulariosController controller = new FormulariosController(mockDb.Object);

            testSetup.SetupHttpContext(controller);

            var result = controller.BorrarPregunta(codSeccion, codPregunta);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestModificarFormulario()
        {
            string codViejo = "00000001";
            string codNuevo = "testmodf";
            string nombre = "trivial";

            var mockDb = new Mock<DataIntegradorEntities>();
            mockDb.Setup(m => m.ModificarFormulario(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ObjectParameter>())).Callback<string, string, string, ObjectParameter>((a, b, c, d) =>
            {
                d.Value = true;
            });

            FormulariosController formularios = new FormulariosController(mockDb.Object);
            var result = formularios.ModificarFormulario(codViejo, codNuevo, nombre) as JsonResult;

            Assert.AreEqual("{ modificacionExitosa = True }", result.Data.ToString());
        }

        [TestMethod]
        public void TestModificarFormularioNulo()
        {
            FormulariosController formularios = new FormulariosController();
            var result = formularios.ModificarFormulario(null, null, null) as JsonResult;

            Assert.AreEqual("{ modificacionExitosa = False }", result.Data.ToString());
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
