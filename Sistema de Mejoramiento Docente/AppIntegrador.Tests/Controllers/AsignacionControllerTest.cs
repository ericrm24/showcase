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
using System.Globalization;

namespace AppIntegrador.Tests.Controllers
{
    [TestClass]
    public class AsignacionControllerTest
    {
        // Constructor
        public AsignacionControllerTest()
        { 

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
        // Prueba que valida la redirección del sitio,cuando no hay un id de formulario
        // Pruba Integración
        public void TestIndexNotNullWithoutId()
        {
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            RedirectToRouteResult result = asignacionController.Index(null) as RedirectToRouteResult;
            Assert.IsNotNull(result);
        }


        [TestMethod]
        // Prueba que la vista no sea nula dada un código de formulario
        // Prueba integración
        public void TestIndexNotNullWithId()
        {
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            ActionResult result = asignacionController.Index("00000001") as ActionResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        // Prueba el metodo que divide las carrera/enfasis, en dos separados
        public void TestDividirCarreraEnfasisNull()
        {
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            // Para poder acceder a los métodos privados
            MethodInfo methodInfo = typeof(AsignacionFormulariosController).GetMethod("DividirCarreraEnfasis", BindingFlags.NonPublic | BindingFlags.Instance);
            // Parametros del llamado
            object[] parameters = { "null", "null", "null" };
            // Invocacion del metodo
            var returnvalue = methodInfo.Invoke(asignacionController, parameters);

            Assert.AreEqual(returnvalue, false);
        }

        [TestMethod]
        // Prueba de unidad, que verifica el llamado
        // al metodo DividirCarreraEnfasis con valores
        // de prueba veridicos, y valida que divida las strings de la manera adecuada
        public void TestDividirCarreraEnfasisNotNull()
        {
            string codigoCarrera = "000001";
            string codigoEnfasis = "000002";
            string codigoCarreraEnfasis = codigoCarrera + "/" + codigoEnfasis;

            string codigoCarreraResultado = null;
            string codigoEnfasisResultado = null;
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            // Para poder acceder a los métodos privados
            MethodInfo methodInfo = typeof(AsignacionFormulariosController).GetMethod("DividirCarreraEnfasis", BindingFlags.NonPublic | BindingFlags.Instance);
            // Parametros del llamado
            object[] parameters = {  codigoCarreraEnfasis,  codigoCarreraResultado,  codigoEnfasisResultado };
            // Invocacion del metodo
            var returnvalue = methodInfo.Invoke(asignacionController, parameters);

            // Se recuperan los valores que modifica la función
            codigoCarreraResultado = (string)parameters[1];
            codigoEnfasisResultado = (string)parameters[2];

            Assert.AreEqual(codigoCarrera, codigoCarreraResultado);
            Assert.AreEqual(codigoEnfasis, codigoEnfasisResultado);
        }

        [TestMethod]
        // Prueba el metodo que divide las grupo, extrayendo de un string
        // su numero grupo, anno, periodo, codigo pero con datos de entrada nulos
        public void TestDividirGrupoNull()
        {
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            // Para poder acceder a los métodos privados
            MethodInfo methodInfo = typeof(AsignacionFormulariosController).GetMethod("DividirGrupo", BindingFlags.NonPublic | BindingFlags.Instance);
            // Parametros del llamado nulos
            object[] parameters = { "null", null,null,null,null};
            // Invocacion del metodo

            var returnvalue = methodInfo.Invoke(asignacionController, parameters);

            Assert.AreEqual(returnvalue, false);
        }

        [TestMethod]
        // Prueba de unidad, que verifica el llamado
        // al metodo DividirCarreraEnfasis con valores
        // de prueba veridicos, y valida que divida las strings de la manera adecuada
        public void TestDividirGrupoNotNull()
        {
            string siglaCursoGrupoSeleccionado = "CI-0128";
            byte numeroGrupoSeleccionado = 1;
            byte semestreGrupoSeleccionado = 2;
            int annoSeleccioado = 2019;
            // Concatenación del grupo
            string grupoSeleccionado = siglaCursoGrupoSeleccionado + "/" + numeroGrupoSeleccionado
                + "/" + semestreGrupoSeleccionado + "/" + annoSeleccioado;
            // Variables para comparar
            string grupoSeleccionadoResultado = null;
            string siglaCursoGrupoResultado = null;
            Nullable<byte> numeroGrupoResultado = null;
            Nullable<byte> semestreGrupoResultado = null;
            Nullable<int> annoResultado = null;
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            // Para poder acceder a los métodos privados
            MethodInfo methodInfo = typeof(AsignacionFormulariosController).GetMethod("DividirGrupo", BindingFlags.NonPublic | BindingFlags.Instance);
            // Parametros del llamado
            object[] parameters = { grupoSeleccionado, siglaCursoGrupoResultado, numeroGrupoResultado, semestreGrupoResultado, annoResultado };
            // Invocacion del metodo
            var returnvalue = methodInfo.Invoke(asignacionController, parameters);

            // Se recuperan los valores que modifica la función
            siglaCursoGrupoResultado = (string)parameters[1];
            numeroGrupoResultado = (byte)parameters[2];
            semestreGrupoResultado = (byte)parameters[3];
            annoResultado = (int)parameters[4];

            // Compara que la división se hiciera correctamente.
            Assert.AreEqual(siglaCursoGrupoSeleccionado, siglaCursoGrupoResultado);
            Assert.AreEqual(numeroGrupoSeleccionado, numeroGrupoResultado);
            Assert.AreEqual(semestreGrupoSeleccionado, semestreGrupoResultado);
            Assert.AreEqual(annoSeleccioado, annoResultado);
 
        }

        [TestMethod]
        // Prueba de integración para asignar un formulario por unidad academica
        public void TestAsignarFormularioUnidadAcademica()
        {
            string codigoFormulario = "00000001";
            string codigoUASeleccionada = "0000000001";
            string codigoCarreraEnfasisSeleccionada = "null";
            string grupoSeleccionado = "null";
            string correoProfesorSeleccionado = null;
            string fechaInicioSeleccionado = "2020-10-21";
            string fechaFinSeleccionado = "2021-10-21";
            bool extenderPeriodo = false;
            bool enviarCorreos = false;
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            JsonResult result = asignacionController.Asignar(codigoFormulario, codigoUASeleccionada,
                codigoCarreraEnfasisSeleccionada, grupoSeleccionado,
                correoProfesorSeleccionado, fechaInicioSeleccionado,
                fechaFinSeleccionado, extenderPeriodo, enviarCorreos) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        // Prueba de integración para asignar un formulario por unidad academica
        public void TestAsignarFormularioNull()
        {
            string codigoFormulario = "null";
            string codigoUASeleccionada = "null";
            string codigoCarreraEnfasisSeleccionada = "null";
            string grupoSeleccionado = "null";
            string correoProfesorSeleccionado = null;
            string fechaInicioSeleccionado = "2020-10-21";
            string fechaFinSeleccionado = "2021-10-21";
            bool extenderPeriodo = false;
            bool enviarCorreos = false;
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            JsonResult result = asignacionController.Asignar(codigoFormulario, codigoUASeleccionada,
                codigoCarreraEnfasisSeleccionada, grupoSeleccionado,
                correoProfesorSeleccionado, fechaInicioSeleccionado,
                fechaFinSeleccionado, extenderPeriodo, enviarCorreos) as JsonResult;

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void TestAsignarFormularioSinGruposAsociados()
        {
            string codigoFormulario = "00000001";
            string codigoUASeleccionada = "00000000";
            string codigoCarreraEnfasisSeleccionada = "null";
            string grupoSeleccionado = "null";
            string correoProfesorSeleccionado = null;
            string fechaInicioSeleccionado = "2020-10-21";
            string fechaFinSeleccionado = "2021-10-21";
            bool extenderPeriodo = false;
            bool enviarCorreos = false;
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            JsonResult result = asignacionController.Asignar(codigoFormulario, codigoUASeleccionada,
                codigoCarreraEnfasisSeleccionada, grupoSeleccionado,
                correoProfesorSeleccionado, fechaInicioSeleccionado,
                fechaFinSeleccionado, extenderPeriodo, enviarCorreos) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        // Prueba de integración para asignar un formulario por unidad academica
        public void TestAsignarFormularioFechaInicioMayorFin()
        {
            string codigoFormulario = "^^000001";
            string codigoUASeleccionada = "null";
            string codigoCarreraEnfasisSeleccionada = "null";
            string grupoSeleccionado = "null";
            string correoProfesorSeleccionado = null;
            string fechaInicioSeleccionado = "2021-10-21";
            string fechaFinSeleccionado = "2020-12-21";
            bool extenderPeriodo = false;
            bool enviarCorreos = false;
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            JsonResult result = asignacionController.Asignar(codigoFormulario, codigoUASeleccionada,
                codigoCarreraEnfasisSeleccionada, grupoSeleccionado,
                correoProfesorSeleccionado, fechaInicioSeleccionado,
                fechaFinSeleccionado, extenderPeriodo, enviarCorreos) as JsonResult;

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

        [TestMethod]
        public void ProbarAsignacionNoSolapada()
        {
            AsignacionFormulariosController controller = new AsignacionFormulariosController();

            DateTime inicio = DateTime.ParseExact("25/12/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime fin = DateTime.ParseExact("26/12/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            FechasSolapadasInfo fechas = controller.VerificarFechasSolapadas("00000001", 2019, 2, 1, inicio, fin);

            Assert.IsNull(fechas);
        }

        [TestMethod]
        public void ProbarAsignacionSolapadaInicio()
        {
            AsignacionFormulariosController controller = new AsignacionFormulariosController();

            DateTime inicio = DateTime.ParseExact("05/06/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime fin = DateTime.ParseExact("10/06/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            FechasSolapadasInfo fechas = controller.VerificarFechasSolapadas("00000001", 2019, 2, 1, inicio, fin);

            Assert.IsNotNull(fechas.FechaInicioNueva);
        }

        [TestMethod]
        public void ProbarAsignacionSolapadaFin()
        {
            AsignacionFormulariosController controller = new AsignacionFormulariosController();

            DateTime inicio = DateTime.ParseExact("10/06/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime fin = DateTime.ParseExact("15/06/2019", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            FechasSolapadasInfo fechas = controller.VerificarFechasSolapadas("00000001", 2019, 2, 1, inicio, fin);

            Assert.IsNotNull(fechas.FechaFinNueva);
        }

        [TestMethod]
        public void ProbarAsignacionSolapadaConfirmada()
        {
            string codigoFormulario = "00000001";
            string codigoUASeleccionada = "0000000001";
            string codigoCarreraEnfasisSeleccionada = "null";
            string grupoSeleccionado = "null";
            string correoProfesorSeleccionado = null;
            string fechaInicioSeleccionado = "2020-06-10";
            string fechaFinSeleccionado = "2020-06-15";
            bool extenderPeriodo = true;
            bool enviarCorreos = false;
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            JsonResult result = asignacionController.Asignar(codigoFormulario, codigoUASeleccionada,
                codigoCarreraEnfasisSeleccionada, grupoSeleccionado,
                correoProfesorSeleccionado, fechaInicioSeleccionado,
                fechaFinSeleccionado, extenderPeriodo, enviarCorreos) as JsonResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProbarAsignacionSinSolapar()
        {
            string codigoFormulario = "00000001";
            string codigoUASeleccionada = "0000000001";
            string codigoCarreraEnfasisSeleccionada = "null";
            string grupoSeleccionado = "null";
            string correoProfesorSeleccionado = null;
            string fechaInicioSeleccionado = "2020-10-12";
            string fechaFinSeleccionado = "2020-12-15";
            bool extenderPeriodo = false;
            bool enviarCorreos = false;
            AsignacionFormulariosController asignacionController = new AsignacionFormulariosController();
            JsonResult result = asignacionController.Asignar(codigoFormulario, codigoUASeleccionada,
                codigoCarreraEnfasisSeleccionada, grupoSeleccionado,
                correoProfesorSeleccionado, fechaInicioSeleccionado,
                fechaFinSeleccionado, extenderPeriodo, enviarCorreos) as JsonResult;

            Assert.IsNotNull(result);
        }
    }
}
