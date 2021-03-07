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
using System.Web.SessionState;
using System.Reflection;
using System.IO;

namespace AppIntegrador.Tests.Controllers
{
    [TestClass]
    public class VistaPreviaFormularioTest
    {
        [TestMethod]
        public void TestVistaPreviaSinHttpContextDataMock()
        {
            var mockDb = new Mock<DataIntegradorEntities>();
            string codFormulario = "CI0128G2";
            LlenarFormularioController controller = new LlenarFormularioController(mockDb.Object);

            // Act
            var result = controller.VistaPrevia(codFormulario);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestVistaPreviaSinCodigoFormulario()
        {
            TestSetup testSetup = new TestSetup();

            var mockDb = new Mock<DataIntegradorEntities>();
            LlenarFormularioController controller = new LlenarFormularioController(mockDb.Object);

            // Act
            var result = controller.VistaPrevia(null);
            testSetup.SetupHttpContext(controller);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestVistaPreviaSinSeccionesDataMock()
        {
            TestSetup testSetup = new TestSetup();

            var mockDb = new Mock<DataIntegradorEntities>();
            string codFormulario = "CI0128G2";
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = "Formularios de prueba para CI0128"
            };
            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            LlenarFormularioController controller = new LlenarFormularioController(mockDb.Object);

            // Act
            var result = controller.VistaPrevia(codFormulario);
            testSetup.SetupHttpContext(controller);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestVistaPreviaSinPreguntasDataMock()
        {
            TestSetup testSetup = new TestSetup();

            var mockDb = new Mock<DataIntegradorEntities>();
            string codFormulario = "CI0128G2";
            string codSeccion = "12345678";
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = "Formularios de prueba para CI0128"
            };
            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            ObtenerSeccionesDeFormulario_Result seccion = new ObtenerSeccionesDeFormulario_Result
            {
                Codigo = codSeccion,
                Nombre = "Sección sobre algo",
                Orden = 0
            };

            var mockedObtenerSecciones = testSetup.SetupMockProcedure<ObtenerSeccionesDeFormulario_Result>
                (new List<ObtenerSeccionesDeFormulario_Result> { seccion });
            mockDb.Setup(x => x.ObtenerSeccionesDeFormulario(codFormulario)).Returns(mockedObtenerSecciones.Object);

            LlenarFormularioController controller = new LlenarFormularioController(mockDb.Object);

            var result = controller.VistaPrevia(codFormulario);

            testSetup.SetupHttpContext(controller);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestVistaPreviaConPreguntasEscalarSinRespuestaGuardadaDataMock()
        {
            TestSetup testSetup = new TestSetup();

            var mockDb = new Mock<DataIntegradorEntities>();
            string codFormulario = "CI0128G2";
            string codSeccion = "12345678";
            string codPregunta = "escalar";
            Formulario formulario = new Formulario() { Codigo = codFormulario, Nombre = "Formularios de prueba para CI0128" };
            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            ObtenerSeccionesDeFormulario_Result seccion = new ObtenerSeccionesDeFormulario_Result
            {
                Codigo = codSeccion,
                Nombre = "Sección sobre algo",
                Orden = 0
            };

            var mockedObtenerSecciones = testSetup.SetupMockProcedure<ObtenerSeccionesDeFormulario_Result>
                (new List<ObtenerSeccionesDeFormulario_Result> { seccion });
            mockDb.Setup(x => x.ObtenerSeccionesDeFormulario(codFormulario)).Returns(mockedObtenerSecciones.Object);

            ObtenerPreguntasDeSeccion_Result pregunta = new ObtenerPreguntasDeSeccion_Result
            {
                Codigo = codPregunta,
                Enunciado = "¿Cómo calificaría este curso?",
                Tipo = "E",
                Orden = 0
            };
            var mockedObtenerPreguntas = testSetup.SetupMockProcedure<ObtenerPreguntasDeSeccion_Result>
                (new List<ObtenerPreguntasDeSeccion_Result> { pregunta });
            mockDb.Setup(x => x.ObtenerPreguntasDeSeccion(codSeccion)).Returns(mockedObtenerPreguntas.Object);

            Pregunta_con_opciones pregunta_Con_Opciones = new Pregunta_con_opciones
            {
                Codigo = codPregunta,
                TituloCampoObservacion = "¿Por qué?"
            };
            mockDb.Setup(x => x.Pregunta_con_opciones.Find(codPregunta)).Returns(pregunta_Con_Opciones);

            Escalar escalar = new Escalar
            {
                Codigo = codPregunta,
                Incremento = 1,
                Minimo = 1,
                Maximo = 10
            };
            mockDb.Setup(x => x.Escalar.Find(codPregunta)).Returns(escalar);

            LlenarFormularioController controller = new LlenarFormularioController(mockDb.Object);

            testSetup.SetupHttpContext(controller);

            var result = controller.VistaPrevia(codFormulario);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestVistaPreviaConPreguntasEscalarConRespuestaGuardadaDataMock()
        {
            TestSetup testSetup = new TestSetup();

            var mockDb = new Mock<DataIntegradorEntities>();
            string codFormulario = "CI0128G2";
            string codSeccion = "12345678";
            string codPregunta = "escalar";
            Formulario formulario = new Formulario() { Codigo = codFormulario, Nombre = "Formularios de prueba para CI0128" };
            mockDb.Setup(m => m.Formulario.Find(codFormulario)).Returns(formulario);

            ObtenerSeccionesDeFormulario_Result seccion = new ObtenerSeccionesDeFormulario_Result
            {
                Codigo = codSeccion,
                Nombre = "Sección sobre algo",
                Orden = 0
            };

            var mockedObtenerSecciones = testSetup.SetupMockProcedure<ObtenerSeccionesDeFormulario_Result>
                (new List<ObtenerSeccionesDeFormulario_Result> { seccion });
            mockDb.Setup(x => x.ObtenerSeccionesDeFormulario(codFormulario)).Returns(mockedObtenerSecciones.Object);

            ObtenerRespuestasAFormulario_Result respuestas = new ObtenerRespuestasAFormulario_Result
            {
                Correo = "admin@mail.com",
                CSigla = "CI0128",
                GNumero = 1,
                GAnno = 2019,
                GSemestre = 2,
                FCodigo = codFormulario,
            };

            // Se prepara el retorno del procedimiento almacenado en el mock
            var mockedObtenerRespuestas = testSetup.SetupMockProcedure<ObtenerRespuestasAFormulario_Result>
                (new List<ObtenerRespuestasAFormulario_Result> { respuestas });
            mockDb.Setup(x => x.ObtenerRespuestasAFormulario(respuestas.FCodigo, respuestas.Correo, respuestas.CSigla, respuestas.GNumero,
                respuestas.GAnno, respuestas.GSemestre)).Returns(mockedObtenerRespuestas.Object);

            ObtenerPreguntasDeSeccion_Result pregunta = new ObtenerPreguntasDeSeccion_Result
            {
                Codigo = codPregunta,
                Enunciado = "¿Cómo calificaría este curso?",
                Tipo = "E",
                Orden = 0
            };
            var mockedObtenerPreguntas = testSetup.SetupMockProcedure<ObtenerPreguntasDeSeccion_Result>
                (new List<ObtenerPreguntasDeSeccion_Result> { pregunta });
            mockDb.Setup(x => x.ObtenerPreguntasDeSeccion(codSeccion)).Returns(mockedObtenerPreguntas.Object);

            Pregunta_con_opciones pregunta_Con_Opciones = new Pregunta_con_opciones
            {
                Codigo = codPregunta,
                TituloCampoObservacion = "¿Por qué?"
            };
            mockDb.Setup(x => x.Pregunta_con_opciones.Find(codPregunta)).Returns(pregunta_Con_Opciones);

            Escalar escalar = new Escalar
            {
                Codigo = codPregunta,
                Incremento = 1,
                Minimo = 1,
                Maximo = 10
            };
            mockDb.Setup(x => x.Escalar.Find(codPregunta)).Returns(escalar);

            ObtenerRespuestasAPreguntaConOpciones_Result obtenerRespuestasAPreguntaConOpciones = new ObtenerRespuestasAPreguntaConOpciones_Result
            {
                Correo = respuestas.Correo,
                FCodigo = respuestas.FCodigo,
                CSigla = respuestas.CSigla,
                GNumero = respuestas.GNumero,
                GAnno = respuestas.GAnno,
                GSemestre = respuestas.GSemestre,
                SCodigo = codSeccion,
                PCodigo = codPregunta,
                Justificacion = "Porque sí."
            };
            var mockedRespuestaPreguntaConOpciones = testSetup.SetupMockProcedure<ObtenerRespuestasAPreguntaConOpciones_Result>(new List<ObtenerRespuestasAPreguntaConOpciones_Result> { obtenerRespuestasAPreguntaConOpciones });
            mockDb.Setup(x => x.ObtenerRespuestasAPreguntaConOpciones(obtenerRespuestasAPreguntaConOpciones.FCodigo, obtenerRespuestasAPreguntaConOpciones.Correo,
                obtenerRespuestasAPreguntaConOpciones.CSigla, obtenerRespuestasAPreguntaConOpciones.GNumero, obtenerRespuestasAPreguntaConOpciones.GSemestre, obtenerRespuestasAPreguntaConOpciones.GAnno,
                obtenerRespuestasAPreguntaConOpciones.SCodigo, obtenerRespuestasAPreguntaConOpciones.PCodigo)).Returns(mockedRespuestaPreguntaConOpciones.Object);

            ObtenerOpcionesSeleccionadas_Result obtenerOpciones = new ObtenerOpcionesSeleccionadas_Result
            {
                Correo = respuestas.Correo,
                FCodigo = respuestas.FCodigo,
                CSigla = respuestas.CSigla,
                GNumero = respuestas.GNumero,
                GAnno = respuestas.GAnno,
                GSemestre = respuestas.GSemestre,
                SCodigo = codSeccion,
                PCodigo = codPregunta,
                OpcionSeleccionada = 0
            };
            var mockedObtenerOpciones = testSetup.SetupMockProcedure<ObtenerOpcionesSeleccionadas_Result>(new List<ObtenerOpcionesSeleccionadas_Result> { obtenerOpciones });
            mockDb.Setup(x => x.ObtenerOpcionesSeleccionadas(obtenerOpciones.FCodigo, obtenerOpciones.Correo,
                obtenerOpciones.CSigla, obtenerOpciones.GNumero, obtenerOpciones.GSemestre, obtenerOpciones.GAnno,
                obtenerOpciones.SCodigo, obtenerOpciones.PCodigo)).Returns(mockedObtenerOpciones.Object);

            LlenarFormularioController controller = new LlenarFormularioController(mockDb.Object);

            testSetup.SetupHttpContext(controller);

            var result = controller.VistaPrevia(codFormulario);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestVistaPreviaConPreguntasDeOpcionConOpcionesNulas()
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

            LlenarFormularioController controller = new LlenarFormularioController(mockDb.Object);

            testSetup.SetupHttpContext(controller);

            var result = controller.VistaPrevia(codFormulario);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestVistaPreviaConPreguntasDeOpcionUnica()
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
            var mockedObtenerPreguntas = testSetup.SetupMockProcedure<ObtenerPreguntasDeSeccion_Result>
                (new List<ObtenerPreguntasDeSeccion_Result> { pregunta });
            mockDb.Setup(x => x.ObtenerPreguntasDeSeccion(codSeccion)).Returns(mockedObtenerPreguntas.Object);

            Pregunta_con_opciones pregunta_Con_Opciones = new Pregunta_con_opciones
            {
                Codigo = codPregunta,
                Pregunta_con_opciones_de_seleccion = new Pregunta_con_opciones_de_seleccion()
            };
            mockDb.Setup(x => x.Pregunta_con_opciones.Find(codPregunta)).Returns(pregunta_Con_Opciones);

            var mockedOpciones = testSetup.SetupMockProcedure<ObtenerOpcionesDePregunta_Result>(new List<ObtenerOpcionesDePregunta_Result>
            {
                new ObtenerOpcionesDePregunta_Result { Orden = 0, Texto ="A" },
                new ObtenerOpcionesDePregunta_Result { Orden = 1, Texto ="B" },
                new ObtenerOpcionesDePregunta_Result { Orden = 2, Texto ="C" },
                new ObtenerOpcionesDePregunta_Result { Orden = 3, Texto ="D" }
            });
            mockDb.Setup(x => x.ObtenerOpcionesDePregunta(codPregunta)).Returns(mockedOpciones.Object);

            LlenarFormularioController controller = new LlenarFormularioController(mockDb.Object);

            testSetup.SetupHttpContext(controller);

            var result = controller.VistaPrevia(codFormulario);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestVistaPreviaConPreguntaRespuestaLibre()
        {
            TestSetup testSetup = new TestSetup();

            var mockDb = new Mock<DataIntegradorEntities>();

            string codFormulario = "TESTPRL";
            string codSeccion = "SECCPRL";
            string codPregunta = "PREGRL";

            // Se crea el formulario de prueba
            Formulario formulario = new Formulario()
            {
                Codigo = codFormulario,
                Nombre = "Formulario de prueba con preguntas de respuesta libre"
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
                Enunciado = "Explique qué es la ingeniería de los requerimientos.",
                Tipo = "L",
                Orden = 0,
            };
            var mockedObtenerPreguntas = testSetup.SetupMockProcedure<ObtenerPreguntasDeSeccion_Result>
                (new List<ObtenerPreguntasDeSeccion_Result> { pregunta });
            mockDb.Setup(x => x.ObtenerPreguntasDeSeccion(codSeccion)).Returns(mockedObtenerPreguntas.Object);

            ObtenerRespuestasAFormulario_Result respuestas = new ObtenerRespuestasAFormulario_Result
            {
                Correo = "admin@mail.com",
                CSigla = "CI0128",
                GNumero = 1,
                GAnno = 2019,
                GSemestre = 2,
                FCodigo = codFormulario,
            };

            // Se prepara el retorno del procedimiento almacenado en el mock
            var mockedObtenerRespuestas = testSetup.SetupMockProcedure<ObtenerRespuestasAFormulario_Result>
                (new List<ObtenerRespuestasAFormulario_Result> { respuestas });
            mockDb.Setup(x => x.ObtenerRespuestasAFormulario(respuestas.FCodigo, respuestas.Correo, respuestas.CSigla, respuestas.GNumero,
                respuestas.GAnno, respuestas.GSemestre)).Returns(mockedObtenerRespuestas.Object);

            var respuestaLibreList = new List<ObtenerRespuestaLibreGuardada_Result>
            {
                new ObtenerRespuestaLibreGuardada_Result
                {
                    FCodigo = respuestas.FCodigo,
                    Correo = respuestas.Correo,
                    CSigla = respuestas.CSigla,
                    GNumero = respuestas.GNumero,
                    GAnno = respuestas.GAnno,
                    GSemestre = respuestas.GSemestre,
                    SCodigo = codSeccion,
                    PCodigo = codPregunta
                }
            };
            mockDb.Setup(x => x.ObtenerRespuestaLibreGuardada(respuestas.FCodigo, respuestas.Correo, respuestas.CSigla, respuestas.GNumero,
                respuestas.GAnno, respuestas.GSemestre, codPregunta, codSeccion)).Returns(respuestaLibreList.AsQueryable());

            LlenarFormularioController controller = new LlenarFormularioController(mockDb.Object);

            testSetup.SetupHttpContext(controller);

            var result = controller.VistaPrevia(codFormulario);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ProbarVistaPreviaFormularioNula()
        {
            LlenarFormularioController controller = new LlenarFormularioController();
            var result = controller.VistaPrevia(null) as ViewResult;

            Assert.IsNull(result);
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
