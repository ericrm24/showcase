using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using AppIntegrador.Models;
using AppIntegrador.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using System.IO;
using System.Web.Routing;

using System.Reflection;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Security.Principal;


namespace AppIntegrador.Tests.Controllers
{
    [TestClass]
    public class CSVcontrollerTest
    {
        // COD-70: Yo como administrador quiero almacenar los datos de un archivo CSV en el sistema
        // Tarea técnica: Cargar datos en blanco con el dato anteriomente registrado


        /*************** Pruebas de Unidad ******************/


        [TestMethod]
        public void TestIndex()
        {
            // Arrange
            Init("admin@mail.com");
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            CSVController controller = new CSVController();
            // Act
            ViewResult result = controller.Index() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestClase()
        {
            // Arrange
            Init("admin@mail.com");
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            CSVController controller = new CSVController();
            // Act
            ViewResult result = controller.Clase() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestFuncionarios()
        {
            // Arrange
            Init("admin@mail.com");
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            CSVController controller = new CSVController();
            // Act
            ViewResult result = controller.Funcionarios() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGuia()
        {
            // Arrange
            Init("admin@mail.com");
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            CSVController controller = new CSVController();
            // Act
            ViewResult result = controller.GuiaHorarios() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCargarEstudiantes()
        {
            // Arrange
            //Creamos httpContextMock and serverMock:
            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            //Mock el path original
            serverMock.Setup(x => x.MapPath("./ArchivoCSV")).Returns(Path.GetFullPath("../../ArchivoCSVtest"));

            //Mockeamos el objeto HTTPContext.Server con el servidor ya mockeado:
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);

            //Creo el controlador y su contexto
            CSVController controller = new CSVController();
            controller.ControllerContext = new ControllerContext(httpContextMock.Object, new RouteData(), controller);

            //Insertamos el csv 
            string filePath = Path.GetFullPath("../../ArchivoCSVtest/listaEstudianteTest.csv");
            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            //Cargamos el mock
            Mock<HttpPostedFileBase> uploadedFile = new Mock<HttpPostedFileBase>();
            uploadedFile.Setup(x => x.FileName).Returns("listaEstudianteTest.csv");
            uploadedFile.Setup(x => x.ContentType).Returns("listaEstudianteTest.csv");
            uploadedFile.Setup(x => x.InputStream).Returns(fileStream);
            HttpPostedFileBase[] httpPostedFileBases = { uploadedFile.Object };
            // Act
            ViewResult result = controller.Index(httpPostedFileBases[0]) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void TestCargarFuncionarios()
        {
            // Arrange
            //Creamos httpContextMock and serverMock:
            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            //Mock el path original
            serverMock.Setup(x => x.MapPath("./ArchivoCSV")).Returns(Path.GetFullPath("../../ArchivoCSVtest"));

            //Mockeamos el objeto HTTPContext.Server con el servidor ya mockeado:
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);

            //Creo el controlador y su contexto
            CSVController controller = new CSVController();
            controller.ControllerContext = new ControllerContext(httpContextMock.Object, new RouteData(), controller);

            //Insertamos el csv 
            string filePath = Path.GetFullPath("../../ArchivoCSVtest/listaFuncionarioTest.csv");
            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            //Cargamos el mock
            Mock<HttpPostedFileBase> uploadedFile = new Mock<HttpPostedFileBase>();
            uploadedFile.Setup(x => x.FileName).Returns("listaFuncionarioTest.csv");
            uploadedFile.Setup(x => x.ContentType).Returns("listaFuncionarioTest.csv");
            uploadedFile.Setup(x => x.InputStream).Returns(fileStream);
            HttpPostedFileBase[] httpPostedFileBases = { uploadedFile.Object };
            // Act
            ViewResult result = controller.Funcionarios(httpPostedFileBases[0]) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCargarClase()
        {
            // Arrange
            //Creamos httpContextMock and serverMock:
            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            //Mock el path original
            serverMock.Setup(x => x.MapPath("./ArchivoCSV")).Returns(Path.GetFullPath("../../ArchivoCSVtest"));

            //Mockeamos el objeto HTTPContext.Server con el servidor ya mockeado:
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);

            //Creo el controlador y su contexto
            CSVController controller = new CSVController();
            controller.ControllerContext = new ControllerContext(httpContextMock.Object, new RouteData(), controller);

            //Insertamos el csv 
            string filePath = Path.GetFullPath("../../ArchivoCSVtest/listaClaseTest.csv");
            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            //Cargamos el mock
            Mock<HttpPostedFileBase> uploadedFile = new Mock<HttpPostedFileBase>();
            uploadedFile.Setup(x => x.FileName).Returns("listaClaseTest.csv");
            uploadedFile.Setup(x => x.ContentType).Returns("listaClaseTest.csv");
            uploadedFile.Setup(x => x.InputStream).Returns(fileStream);
            HttpPostedFileBase[] httpPostedFileBases = { uploadedFile.Object };
            // Act
            ViewResult result = controller.Clase(httpPostedFileBases[0]) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCargarGuia()
        {
            // Arrange
            //Creamos httpContextMock and serverMock:
            var httpContextMock = new Mock<HttpContextBase>();
            var serverMock = new Mock<HttpServerUtilityBase>();
            //Mock el path original
            serverMock.Setup(x => x.MapPath("./ArchivoCSV")).Returns(Path.GetFullPath("../../ArchivoCSVtest"));

            //Mockeamos el objeto HTTPContext.Server con el servidor ya mockeado:
            httpContextMock.Setup(x => x.Server).Returns(serverMock.Object);

            //Creo el controlador y su contexto
            CSVController controller = new CSVController();
            controller.ControllerContext = new ControllerContext(httpContextMock.Object, new RouteData(), controller);

            //Insertamos el csv 
            string filePath = Path.GetFullPath("../../ArchivoCSVtest/guiaTest.csv");
            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            //Cargamos el mock
            Mock<HttpPostedFileBase> uploadedFile = new Mock<HttpPostedFileBase>();
            uploadedFile.Setup(x => x.FileName).Returns("guiaTest.csv");
            uploadedFile.Setup(x => x.ContentType).Returns("guiaTest.csv");
            uploadedFile.Setup(x => x.InputStream).Returns(fileStream);
            HttpPostedFileBase[] httpPostedFileBases = { uploadedFile.Object };
            // Act
            ViewResult result = controller.GuiaHorarios(httpPostedFileBases[0]) as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }



        /*************** Pruebas de Integracion ******************/


        [TestMethod]
        public void TestCargaGuia()
        {
            // Arrange
            //Creo el controlador y su contexto
            CSVController controller = new CSVController();
            var file = @"../../ArchivoCSVtest/guiaTest.csv";
            var fileMalo = @"../../ArchivoCSVtest/guiaTestMalo.csv";
            // Act
            var result = controller.cargarGuia(file);
            var resultMalo = controller.cargarGuia(fileMalo);
            // Assert
            Assert.IsTrue(result.Item1); //camino feliz
            Assert.IsFalse(resultMalo.Item1); //camino feliz
        }


        [TestMethod]
        public void TestCargaEstudiantes()
        {
            // Arrange
            //Creo el controlador y su contexto
            CSVController controller = new CSVController();
            var file = @"../../ArchivoCSVtest/listaEstudianteTest.csv";
            var fileMalo = @"../../ArchivoCSVtest/listaEstudianteTestMalo.csv";
            // Act
            var result = controller.cargarListaEstudiante(file);
            var resultMalo = controller.cargarListaEstudiante(fileMalo);
            // Assert
            Assert.IsTrue(result.Item1); //camino feliz
            Assert.IsFalse(resultMalo.Item1); //camino feliz
        }

        [TestMethod]
        public void TestCargaFuncionario()
        {
            // Arrange
            //Creo el controlador y su contexto
            CSVController controller = new CSVController();
            var file = @"../../ArchivoCSVtest/listaFuncionarioTest.csv";
            var fileMalo = @"../../ArchivoCSVtest/listaFuncionarioTestMalo.csv";
            // Act
            var result = controller.cargarListaFuncionario(file);
            var resultMalo = controller.cargarListaFuncionario(fileMalo);
            // Assert
            Assert.IsTrue(result.Item1); //camino feliz
            Assert.IsFalse(resultMalo.Item1); //camino feliz
        }

        [TestMethod]
        public void TestCargaClases()
        {
            // Arrange
            //Creo el controlador y su contexto
            CSVController controller = new CSVController();
            var file = @"../../ArchivoCSVtest/listaClaseTest.csv";
            var fileMalo = @"../../ArchivoCSVtest/listaClaseTestMalo.csv";
            // Act
            var result = controller.cargarListaClase(file);
            var resultMalo = controller.cargarListaClase(fileMalo);
            // Assert
            Assert.IsTrue(result.Item1); //camino feliz
            Assert.IsFalse(resultMalo.Item1); //camino feliz
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
    }
}
