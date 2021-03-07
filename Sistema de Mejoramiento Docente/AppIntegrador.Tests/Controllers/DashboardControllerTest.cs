using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppIntegrador.Controllers;
using AppIntegrador.Models;
using System.Web.Mvc;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Security.Principal;
using System.Web;
using System.Reflection;
using System.Web.SessionState;
using System.IO;

namespace AppIntegrador.Tests.Controllers
{
    [TestClass]
    public class DashboardControllerTest
    {

        [TestMethod]
        public void IndexNotNullTest()
        {
            //Arrange
            DashboardController controller = new DashboardController();

            //Act
            ViewResult view = controller.Index() as ViewResult;

            //Assert
            Assert.IsNotNull(view);
        }

        //Este test tiene como propósito corroborar que los resultados de las funciones que implementan los filtros se serializan correctamente.
        [TestMethod]
        public void ObtenerUAsParametrosNulosTest()
        {
            //Arrange

            //Se crea el mock de la base de datos
            var mockDb = new Mock<DataIntegradorEntities>();
            var mockFiltrosDb = new Mock<FiltrosEntities>();

            //Se instancia el controlador y se le pasa como parámetro el mock
            DashboardController controller = new DashboardController(mockDb.Object, mockFiltrosDb.Object);

            //Se crean unidades académicas como dummy data
            var UAsDummy = new List<UAsFiltros>
            {
                new UAsFiltros
                {
                    CodigoUA = "00000001",
                    NombreUA = "ECCI"
                },
                new UAsFiltros
                {
                    CodigoUA = "00000002",
                    NombreUA = "Facultad de Ingeniería"
                }
            };

            //Se hace el mock del procedimiento almacenado que utiliza el método del controlador
            var mockedObjectResult = new Mock<IQueryable<UAsFiltros>>();
            mockedObjectResult.Setup(x => x.GetEnumerator()).Returns(UAsDummy.GetEnumerator());
            mockFiltrosDb.Setup(x => x.ObtenerUAsFiltros(null, null, null)).Returns(mockedObjectResult.Object);

            //Act

            //Se hace el llamado al controlador y se obtiene el JSON
            string uasJson = controller.ObtenerUnidadesAcademicas(null, null, null);

            //Se deserializa el JSON
            var uas = JsonConvert.DeserializeObject<List<UAsFiltros>>(uasJson);

            //Assert

            //Se comparan los miembros del formulario dummy con los del formulario retornado por el controlador
            Assert.IsTrue(CompararUAs(UAsDummy, uas));
        }

        //Este test tiene como propósito corroborar que los resultados de las funciones que implementan los filtros se serializan correctamente.
        [TestMethod]
        public void ObtenerCEsParametrosNulosTest()
        {
            //Arrange

            //Se crea el mock de la base de datos
            var mockDb = new Mock<DataIntegradorEntities>();
            var mockFiltrosDb = new Mock<FiltrosEntities>();

            //Se instancia el controlador y se le pasa como parámetro el mock
            DashboardController controller = new DashboardController(mockDb.Object, mockFiltrosDb.Object);

            //Se crean carreras y énfasis como dummy data
            var CEsDummy = new List<CarrerasEnfasisFiltros>
            {
                new CarrerasEnfasisFiltros
                {
                    CodCarrera = "00000001",
                    NomCarrera = "Bachillerato en Computación",
                    CodEnfasis = "00000001",
                    NomEnfasis = "Tronco Común"
                },
                new CarrerasEnfasisFiltros
                {
                    CodCarrera = "00000001",
                    NomCarrera = "Bachillerato en Computación",
                    CodEnfasis = "00000002",
                    NomEnfasis = "Ciencias de la Computación"
                }
            };

            //Se hace el mock del procedimiento almacenado que utiliza el método del controlador
            var mockedObjectResult = new Mock<IQueryable<CarrerasEnfasisFiltros>>();
            mockedObjectResult.Setup(x => x.GetEnumerator()).Returns(CEsDummy.GetEnumerator());
            mockFiltrosDb.Setup(x => x.ObtenerCarrerasEnfasisFiltros(null, null, null)).Returns(mockedObjectResult.Object);

            //Act

            //Se hace el llamado al controlador y se obtiene el JSON
            string cesJson = controller.ObtenerCarrerasEnfasis(null, null, null);

            //Se deserializa el JSON
            var ces = JsonConvert.DeserializeObject<List<CarrerasEnfasisFiltros>>(cesJson);

            //Assert

            //Se comparan los miembros del formulario dummy con los del formulario retornado por el controlador
            Assert.IsTrue(CompararCEs(CEsDummy, ces));
        }

        //Este test tiene como propósito corroborar que los resultados de las funciones que implementan los filtros se serializan correctamente.
        [TestMethod]
        public void ObtenerGsParametrosNulosTest()
        {
            //Arrange

            //Se crea el mock de la base de datos
            var mockDb = new Mock<DataIntegradorEntities>();
            var mockFiltrosDb = new Mock<FiltrosEntities>();

            //Se instancia el controlador y se le pasa como parámetro el mock
            DashboardController controller = new DashboardController(mockDb.Object, mockFiltrosDb.Object);

            //Se crean cursos y grupos como dummy data
            var gsDummy = new List<GruposFiltros>
            {
                new GruposFiltros
                {
                    SiglaCurso = "CI0128",
                    NombreCurso = "Proyecto Integrador de BD e IS",
                    NumGrupo = 1,
                    Semestre = 2,
                    Anno = 2,
                },
                new GruposFiltros
                {
                    SiglaCurso = "CI0127",
                    NombreCurso = "Bases de datos",
                    NumGrupo = 1,
                    Semestre = 2,
                    Anno = 2,
                }
            };

            //Se hace el mock del procedimiento almacenado que utiliza el método del controlador
            var mockedObjectResult = new Mock<IQueryable<GruposFiltros>>();
            mockedObjectResult.Setup(x => x.GetEnumerator()).Returns(gsDummy.GetEnumerator());
            mockFiltrosDb.Setup(x => x.ObtenerGruposFiltros(null, null, null)).Returns(mockedObjectResult.Object);

            //Act

            //Se hace el llamado al controlador y se obtiene el JSON
            string gsJson = controller.ObtenerGrupos(null, null, null);

            //Se deserializa el JSON
            var gs = JsonConvert.DeserializeObject<List<GruposFiltros>>(gsJson);

            //Assert

            //Se comparan los miembros del formulario dummy con los del formulario retornado por el controlador
            Assert.IsTrue(CompararGs(gsDummy, gs));
        }

        //Este test tiene como propósito corroborar que los resultados de las funciones que implementan los filtros se serializan correctamente.
        [TestMethod]
        public void ObtenerPsParametrosNulosTest()
        {
            //Arrange

            //Se crea el mock de la base de datos
            var mockDb = new Mock<DataIntegradorEntities>();
            var mockFiltrosDb = new Mock<FiltrosEntities>();

            //Se instancia el controlador y se le pasa como parámetro el mock
            DashboardController controller = new DashboardController(mockDb.Object, mockFiltrosDb.Object);

            //Se crean profesores como dummy data
            var PsDummy = new List<ProfesoresFiltros>
            {
                new ProfesoresFiltros
                {
                    Correo = "ismael@mail.com",
                    Nombre1 = "Ismael",
                    Nombre2 = "",
                    Apellido1 = "Gutiérrez",
                    Apellido2 = "Hidalgo"
                },
                new ProfesoresFiltros
                {
                    Correo = "christianasch@mail.com",
                    Nombre1 = "Christian",
                    Nombre2 = "",
                    Apellido1 = "Asch",
                    Apellido2 = "Burgos"
                }
            };

            //Se hace el mock del procedimiento almacenado que utiliza el método del controlador
            var mockedObjectResult = new Mock<IQueryable<ProfesoresFiltros>>();
            mockedObjectResult.Setup(x => x.GetEnumerator()).Returns(PsDummy.GetEnumerator());
            mockFiltrosDb.Setup(x => x.ObtenerProfesoresFiltros(null, null, null)).Returns(mockedObjectResult.Object);

            //Act

            //Se hace el llamado al controlador y se obtiene el JSON
            string psJson = controller.ObtenerProfesores(null, null, null);

            //Se deserializa el JSON
            var ps = JsonConvert.DeserializeObject<List<ProfesoresFiltros>>(psJson);

            //Assert

            //Se comparan los miembros del formulario dummy con los del formulario retornado por el controlador
            Assert.IsTrue(CompararPs(PsDummy, ps));
        }

        //Este test tiene como propósito corroborar que los resultados de las funciones que implementan los filtros se serializan correctamente.
        [TestMethod]
        public void ObtenerFormulariosParametrosNulosTest()
        {
            //Arrange

            //Se crea el mock de la base de datos
            var mockDb = new Mock<DataIntegradorEntities>();
            var mockFiltrosDb = new Mock<FiltrosEntities>();

            //Se instancia el controlador y se le pasa como parámetro el mock
            DashboardController controller = new DashboardController(mockDb.Object, mockFiltrosDb.Object);

            //Se crear un formulario como dummy data
            var formulariosDummy = new List<FormulariosFiltros>
            {
                new FormulariosFiltros
                {
                    FCodigo = "00000001",
                    FNombre = "Formulario de prueba",
                    CSigla = "CI0128",
                    GNumero = 1,
                    GSemestre = 2,
                    GAnno = 2,
                    FechaInicio = DateTime.Parse("09/06/2019"),
                    FechaFin = DateTime.Parse("11/06/2019")
                },
                new FormulariosFiltros
                {
                    FCodigo = "00000002",
                    FNombre = "Formulario de fin de curso",
                    CSigla = "CI0128",
                    GNumero = 1,
                    GSemestre = 2,
                    GAnno = 2,
                    FechaInicio = DateTime.Parse("09/11/2019"),
                    FechaFin = DateTime.Parse("11/11/2019")
                }
            };

            //Se hace el mock del procedimiento almacenado que utiliza el método del controlador
            var mockedObjectResult = new Mock<IQueryable<FormulariosFiltros>>();
            mockedObjectResult.Setup(x => x.GetEnumerator()).Returns(formulariosDummy.GetEnumerator());
            mockFiltrosDb.Setup(x => x.ObtenerFormulariosFiltros(null, null, null, null)).Returns(mockedObjectResult.Object);

            //Act

            //Se hace el llamado al controlador y se obtiene el JSON
            string formulariosJson = controller.ObtenerFormularios(null, null, null, null);

            //Se deserializa el JSON
            var formularios = JsonConvert.DeserializeObject<List<FormulariosFiltros>>(formulariosJson, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

            //Assert

            //Se comparan los miembros del formulario dummy con los del formulario retornado por el controlador
            Assert.IsTrue(CompararFormularios(formulariosDummy, formularios));
        }

        //Este test tiene como propósito corroborar que los resultados de las funciones que implementan los filtros se serializan correctamente.
        [TestMethod]
        public void ObtenerFormulariosParametrosNoExistentesTest()
        {
            //Arrange

            //Se crea el mock de la base de datos
            var mockDb = new Mock<DataIntegradorEntities>();
            var mockFiltrosDb = new Mock<FiltrosEntities>();

            //Se instancia el controlador y se le pasa como parámetro el mock
            DashboardController controller = new DashboardController(mockDb.Object, mockFiltrosDb.Object);

            //Se crean los parámetros del controlador
            var unidadesAcademicas = new List<UAsFiltros> { new UAsFiltros { CodigoUA = "01" } };
            var carrerasEnfasis = new List<CarrerasEnfasisFiltros> { new CarrerasEnfasisFiltros { CodCarrera = "01", CodEnfasis = "01" } };
            var grupos = new List<GruposFiltros> { new GruposFiltros { SiglaCurso = "CI0128", NumGrupo = 1, Semestre = 2, Anno = 2019 } };
            var profesores = new List<ProfesoresFiltros> { new ProfesoresFiltros { Correo = "ismael@mail.com" } };

            //Se crear un formulario como dummy data
            var formulariosDummy = new List<FormulariosFiltros>
            {
                new FormulariosFiltros
                {
                    FCodigo = "00000001",
                    FNombre = "Formulario de prueba",
                    CSigla = "CI0128",
                    GNumero = 1,
                    GSemestre = 2,
                    GAnno = 2,
                    FechaInicio = DateTime.Parse("09/06/2019"),
                    FechaFin = DateTime.Parse("11/06/2019")
                },
                new FormulariosFiltros
                {
                    FCodigo = "00000002",
                    FNombre = "Formulario de fin de curso",
                    CSigla = "CI0128",
                    GNumero = 1,
                    GSemestre = 2,
                    GAnno = 2,
                    FechaInicio = DateTime.Parse("09/11/2019"),
                    FechaFin = DateTime.Parse("11/11/2019")
                }
        };

            //Se hace el mock del procedimiento almacenado que utiliza el método del controlador
            var mockedObjectResult = new Mock<IQueryable<FormulariosFiltros>>();
            mockedObjectResult.Setup(x => x.GetEnumerator()).Returns(formulariosDummy.GetEnumerator());
            mockFiltrosDb.Setup(x => x.ObtenerFormulariosFiltros(null, null, null, null)).Returns(mockedObjectResult.Object);

            //Act

            //Se hace el llamado al controlador y se obtiene el JSON
            string formulariosJson = controller.ObtenerFormularios(unidadesAcademicas, carrerasEnfasis, grupos, profesores);

            //Se deserializa el JSON
            var formularios = JsonConvert.DeserializeObject<List<FormulariosFiltros>>(formulariosJson, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

            //Assert

            //Se comparan los miembros del formulario dummy con los del formulario retornado por el controlador
            Assert.IsFalse(CompararFormularios(formulariosDummy, formularios));
        }

        private bool CompararUAs(List<UAsFiltros> dummyUAs, List<UAsFiltros> controllerUAs)
        {
            bool resultado = dummyUAs.Count() == controllerUAs.Count();
            int indice = 0;
            UAsFiltros dummyUA, controllerUA;

            while (resultado && indice < dummyUAs.Count())
            {
                dummyUA = dummyUAs[indice];
                controllerUA = controllerUAs[indice];

                resultado = dummyUA.CodigoUA.Equals(controllerUA.CodigoUA)
                            && dummyUA.NombreUA.Equals(controllerUA.NombreUA);

                ++indice;
            }

            return resultado;
        }

        private bool CompararCEs(List<CarrerasEnfasisFiltros> dummyCEs, List<CarrerasEnfasisFiltros> controllerCEs)
        {
            bool resultado = dummyCEs.Count() == controllerCEs.Count();
            int indice = 0;
            CarrerasEnfasisFiltros dummyCE, controllerCE;

            while (resultado && indice < dummyCEs.Count())
            {
                dummyCE = dummyCEs[indice];
                controllerCE = controllerCEs[indice];

                resultado = dummyCE.CodCarrera.Equals(controllerCE.CodCarrera)
                            && dummyCE.NomCarrera.Equals(controllerCE.NomCarrera)
                            && dummyCE.CodEnfasis.Equals(controllerCE.CodEnfasis)
                            && dummyCE.NomEnfasis.Equals(controllerCE.NomEnfasis);

                ++indice;
            }

            return resultado;
        }

        private bool CompararGs(List<GruposFiltros> dummyGs, List<GruposFiltros> controllerGs)
        {
            bool resultado = dummyGs.Count() == controllerGs.Count();
            int indice = 0;
            GruposFiltros dummyG, controllerG;

            while (resultado && indice < dummyGs.Count())
            {
                dummyG = dummyGs[indice];
                controllerG = controllerGs[indice];

                resultado = dummyG.SiglaCurso.Equals(controllerG.SiglaCurso)
                            && dummyG.NombreCurso.Equals(controllerG.NombreCurso)
                            && dummyG.NumGrupo.Equals(controllerG.NumGrupo)
                            && dummyG.Semestre.Equals(controllerG.Semestre)
                            && dummyG.Anno.Equals(controllerG.Anno);

                ++indice;
            }

            return resultado;
        }

        private bool CompararPs(List<ProfesoresFiltros> dummyPs, List<ProfesoresFiltros> controllerPs)
        {
            bool resultado = dummyPs.Count() == controllerPs.Count();
            int indice = 0;
            ProfesoresFiltros dummyP, controllerP;

            while (resultado && indice < dummyPs.Count())
            {
                dummyP = dummyPs[indice];
                controllerP = controllerPs[indice];

                resultado = dummyP.Correo.Equals(controllerP.Correo)
                            && dummyP.Nombre1.Equals(controllerP.Nombre1)
                            && dummyP.Nombre2.Equals(controllerP.Nombre2)
                            && dummyP.Apellido1.Equals(controllerP.Apellido1)
                            && dummyP.Apellido2.Equals(controllerP.Apellido2);

                ++indice;
            }

            return resultado;
        }

        private bool CompararFormularios (List<FormulariosFiltros> dummyFormularios, List<FormulariosFiltros> controllerformularios)
        {
            bool resultado = dummyFormularios.Count() == controllerformularios.Count();
            int indice = 0;
            FormulariosFiltros dummyFormulario, controllerFormulario;

            while (resultado && indice < dummyFormularios.Count())
            {
                dummyFormulario = dummyFormularios[indice];
                controllerFormulario = controllerformularios[indice];

                resultado =     dummyFormulario.FCodigo.Equals(controllerFormulario.FCodigo)
                                && dummyFormulario.FNombre.Equals(controllerFormulario.FNombre)
                                && dummyFormulario.CSigla.Equals(controllerFormulario.CSigla)
                                && dummyFormulario.GNumero.Equals(controllerFormulario.GNumero)
                                && dummyFormulario.GSemestre.Equals(controllerFormulario.GSemestre)
                                && dummyFormulario.GAnno.Equals(controllerFormulario.GAnno)
                                && dummyFormulario.FechaInicio.Equals(controllerFormulario.FechaInicio)
                                && dummyFormulario.FechaFin.Equals(controllerFormulario.FechaFin);

                ++indice;
            }

            return resultado;
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
