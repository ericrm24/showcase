using AppIntegrador;
using AppIntegrador.Controllers;
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

//MOS-8 Como Usuario administrativo	quiero poder agregar tipos de objetivos para dar opciones a la hora de crear los objetivos
//Tarea 1: "1. Es necesario agregar un scaffold de las operaciones de CRUD de los tipos de objetivos
//Christian Asch
//Commits: c0d43bd, e4023d4

namespace AppIntegrador.Tests.Controllers
{
    [TestClass]
    public class TipoObjetivos
    {
        [TestMethod]
        public void IndexNotNullTest()
        {
            TipoObjetivosController toc = new TipoObjetivosController();
            var indexResult = toc.Index();
            Assert.IsNotNull(indexResult);
        }
        [TestMethod]
        public void IndexNameTest()
        {
            var toc = new TipoObjetivosController();
            var indexResult = toc.Index() as ViewResult;
            Assert.AreEqual("Index", indexResult.ViewName);
            toc.Dispose();
        }

        [TestMethod]
        public void CreateNotNullTest()
        {
            var controller = new TipoObjetivosController();
            var result = controller.Create() as ViewResult;
            Assert.IsNotNull(result);
            controller.Dispose();
        }

        [TestMethod]
        public void CreateNameTest()
        {
            var controller = new TipoObjetivosController();
            var result = controller.Create() as ViewResult;
            Assert.AreEqual(result.ViewName, "Crear");
            controller.Dispose();
        }

        [TestMethod]
        public void CreateDataMockTest()
        {
            var mockdb = new Mock<DataIntegradorEntities>();
            String nombre = "Curso";
            TipoObjetivo to = new TipoObjetivo() { nombre = nombre };

            mockdb.Setup(m => m.TipoObjetivo.Add(to));
            mockdb.Setup(m => m.SaveChanges());

            var controller = new TipoObjetivosController(mockdb.Object);
            var result = controller.Create(to);
            Assert.IsNotNull(result);
            controller.Dispose();
        }

        [TestMethod]
        public void CreateIntegrationTest()
        {
            TipoObjetivo tipoObjetivo = new TipoObjetivo();
            tipoObjetivo.nombre = "ParaPruebas";
            var controller = new TipoObjetivosController();
            var result = controller.Create(tipoObjetivo);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void EditNotNullTest()
        {
            var controller = new TipoObjetivosController();
            String nombre = "Curso";
            var result = controller.Edit(nombre) as ViewResult;
            Assert.IsNotNull(result);
            controller.Dispose();
        }

        [TestMethod]
        public void EditNameTest()
        {
            var controller = new TipoObjetivosController();
            String nombre = "Curso";
            var result = controller.Edit(nombre) as ViewResult;
            Assert.AreEqual(result.ViewName, "Editar");
            controller.Dispose();
        }

        [TestMethod]
        public void EditDataMockTest()
        {
            var mockdb = new Mock<DataIntegradorEntities>();
            String nombre = "Curso";
            TipoObjetivo to = new TipoObjetivo() { nombre = nombre };
            mockdb.Setup(m => m.TipoObjetivo.Find(nombre)).Returns(to);

            var controller = new TipoObjetivosController(mockdb.Object);
            var result = controller.Edit(nombre) as ViewResult;
            Assert.AreEqual(result.Model, to);
            controller.Dispose();
        }

        [TestMethod]
        public void DeleteNotNullTest()
        {
            var controller = new TipoObjetivosController();
            String nombre = "Curso";
            var result = controller.Delete(nombre) as ViewResult;
            Assert.IsNotNull(result);
            controller.Dispose();
        }

        [TestMethod]
        public void DeleteNameTest()
        {
            var controller = new TipoObjetivosController();
            String nombre = "Curso";
            var result = controller.Delete(nombre) as ViewResult;
            Assert.AreEqual(result.ViewName, "Eliminar");
            controller.Dispose();
        }

        [TestMethod]
        public void DeleteDataMockTest()
        {
            var mockdb = new Mock<DataIntegradorEntities>();
            String nombre = "Curso";
            TipoObjetivo to = new TipoObjetivo() { nombre = nombre };
            mockdb.Setup(m => m.TipoObjetivo.Find(nombre)).Returns(to);

            var controller = new TipoObjetivosController(mockdb.Object);
            var result = controller.Delete(nombre) as ViewResult;
            Assert.AreEqual(result.Model, to);
            controller.Dispose();
        }

        [TestMethod]
        public void DeleteConfirmedDataMockTest()
        {
            var mockdb = new Mock<DataIntegradorEntities>();
            String nombre = "Curso";
            TipoObjetivo to = new TipoObjetivo() { nombre = nombre };
            mockdb.Setup(m => m.TipoObjetivo.Find(nombre)).Returns(to);

            var controller = new TipoObjetivosController(mockdb.Object);
            var result = controller.DeleteConfirmed(nombre, true);
            Assert.IsNotNull(result);
            controller.Dispose();
        }

        [TestMethod]
        public void DeleteConfirmedIntegrationTest()
        {
            String nombre = "ParaPruebas";

            var controller = new TipoObjetivosController();
            var result = controller.DeleteConfirmed(nombre, true);
            Assert.IsNotNull(result);
            controller.Dispose();
        }

        //[TestMethod]
        //public void IndexAtLeastThreeTest()
        //{
        //    var toc = new TipoObjetivosController();
        //    var indexResult = toc.Index() as ViewResult;
        //    Assert.IsTrue(indexResult. >= 3);
        //}

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