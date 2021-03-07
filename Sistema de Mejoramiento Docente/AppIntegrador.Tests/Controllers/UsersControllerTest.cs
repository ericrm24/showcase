using AppIntegrador.Controllers;
using AppIntegrador.Models;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Security.Principal;
using System.IO;
using Moq;
using System.Web.SessionState;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace AppIntegrador.Tests.Controllers
{
    [TestClass]
    public class UsersControllerTest
    {
        [TestMethod]
        public void UsersIndexNotNull()
        {
            UsersController controller = new UsersController();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersIndexResultName()
        {
            UsersController controller = new UsersController();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = controller.Index() as ViewResult;

            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void UserCreateNotNull()
        {
            UsersController controller = new UsersController();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateNoPerms()
        {
            UsersController controller = new UsersController();
            CurrentUser.setCurrentUser("tamales@mail.com", "Estudiante", "0000000001", "0000000001");

            RedirectToRouteResult result = controller.Create() as RedirectToRouteResult;

            CurrentUser.deleteCurrentUser("tamales@mail.com");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UsersCreateResultName()
        {
            UsersController controller = new UsersController();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = controller.Create() as ViewResult;

            Assert.AreEqual("Create", result.ViewName);
        }

        [TestMethod]
        public void DetailsNotNull()
        {
            UsersController controller = new UsersController();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = controller.Details("admin@", "mail.com") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DetailsNoPerms()
        {
            UsersController controller = new UsersController();
            CurrentUser.setCurrentUser("tamales@mail.com", "Estudiante", "0000000001", "0000000001");

            RedirectToRouteResult result = controller.Details("admin@", "mail.com") as RedirectToRouteResult;

            CurrentUser.deleteCurrentUser("tamales@mail.com");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void EditNotNull()
        {
            UsersController controller = new UsersController();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            ViewResult result = controller.Edit("admin@", "mail.com") as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void EditNoPerms()
        {
            UsersController controller = new UsersController();
            CurrentUser.setCurrentUser("tamales@mail.com", "Estudiante", "0000000001", "0000000001");

            RedirectToRouteResult result = controller.Edit("admin@", "mail.com") as RedirectToRouteResult;

            CurrentUser.deleteCurrentUser("tamales@mail.com");

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateChangesSaved()
        {
            //TestSetup();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake2@mail.com", Identificacion = "123456782", Apellido1 = "Fake2", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake3@mail.com", Identificacion = "123456783", Apellido1 = "Fake3", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());

            var myMockedObjectResult = new Mock<ObjectResult<int>>();

            Persona nuevaPersona = new Persona();
            nuevaPersona.Correo = "newusertest@mail.com";
            nuevaPersona.Nombre1 = "Test";
            nuevaPersona.Apellido1 = "Nuevo";
            nuevaPersona.TipoIdentificacion = "Cédula";
            nuevaPersona.Identificacion = "120540712";
            nuevaPersona.Estudiante = new Estudiante();

            ObjectParameter parameter = new ObjectParameter("result", typeof(bool));

            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);
            database.Setup(m => m.CheckID(It.IsAny<string>(), It.IsAny<ObjectParameter>())).Callback<string, ObjectParameter>((a, b) =>
            {
                b.Value = false;
            });

            UsersController controller = new UsersController(database.Object);

            var result = controller.Create(nuevaPersona) as RedirectToRouteResult;

            string expected = "";
            if (CurrentUser.getUsername() == "admin@mail.com")
                expected = "Index";
            else
                expected = "../Home/Index";

            Assert.AreEqual(expected, result.RouteValues["action"]);
        }

        [TestMethod]
        public void CreateDeletedUser()
        {
            //TestSetup();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula", Borrado = true },
                new Persona() { Correo = "fake2@mail.com", Identificacion = "123456782", Apellido1 = "Fake2", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake3@mail.com", Identificacion = "123456783", Apellido1 = "Fake3", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());

            var myMockedObjectResult = new Mock<ObjectResult<int>>();

            Persona nuevaPersona = new Persona();
            nuevaPersona.Correo = "fake1@mail.com";
            nuevaPersona.Nombre1 = "Test";
            nuevaPersona.Apellido1 = "Nuevo";
            nuevaPersona.TipoIdentificacion = "Cédula";
            nuevaPersona.Identificacion = "120540712";
            nuevaPersona.Estudiante = new Estudiante();

            ObjectParameter parameter = new ObjectParameter("result", typeof(bool));

            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);
            database.Setup(m => m.CheckID(It.IsAny<string>(), It.IsAny<ObjectParameter>())).Callback<string, ObjectParameter>((a, b) =>
            {
                b.Value = false;
            });

            UsersController controller = new UsersController(database.Object);

            var result = controller.Create(nuevaPersona) as RedirectToRouteResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateRepeatedEmail()
        {
            //TestSetup();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake2@mail.com", Identificacion = "123456782", Apellido1 = "Fake2", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake3@mail.com", Identificacion = "123456783", Apellido1 = "Fake3", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());

            var myMockedObjectResult = new Mock<ObjectResult<int>>();

            Persona nuevaPersona = new Persona();
            nuevaPersona.Correo = "fake1@mail.com";
            nuevaPersona.Nombre1 = "Test";
            nuevaPersona.Apellido1 = "Nuevo";
            nuevaPersona.TipoIdentificacion = "Cédula";
            nuevaPersona.Identificacion = "120540712";
            nuevaPersona.Estudiante = new Estudiante();

            ObjectParameter parameter = new ObjectParameter("result", typeof(bool));

            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);
            database.Setup(m => m.CheckID(It.IsAny<string>(), It.IsAny<ObjectParameter>())).Callback<string, ObjectParameter>((a, b) =>
            {
                b.Value = false;
            });

            UsersController controller = new UsersController(database.Object);

            var result = controller.Create(nuevaPersona) as RedirectToRouteResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateRepeatedID()
        {
            //TestSetup();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake2@mail.com", Identificacion = "123456782", Apellido1 = "Fake2", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake3@mail.com", Identificacion = "123456783", Apellido1 = "Fake3", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());

            var myMockedObjectResult = new Mock<ObjectResult<int>>();

            Persona nuevaPersona = new Persona();
            nuevaPersona.Correo = "fake4@mail.com";
            nuevaPersona.Nombre1 = "Test";
            nuevaPersona.Apellido1 = "Nuevo";
            nuevaPersona.TipoIdentificacion = "Cédula";
            nuevaPersona.Identificacion = "123456781";
            nuevaPersona.Estudiante = new Estudiante();

            ObjectParameter parameter = new ObjectParameter("result", typeof(bool));

            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);
            database.Setup(m => m.CheckID(It.IsAny<string>(), It.IsAny<ObjectParameter>())).Callback<string, ObjectParameter>((a, b) =>
            {
                b.Value = true;
            });

            UsersController controller = new UsersController(database.Object);

            var result = controller.Create(nuevaPersona) as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateSinPermiso()
        {
            //Nos aseguramos que admin no haya quedado logeado por otros tests.
            CurrentUser.deleteCurrentUser("admin@mail.com");

            CurrentUser.setCurrentUser("andres@mail.com", "Estudiante", "0000000001", "0000000001");
            var httpContext = new HttpContext(
                new HttpRequest("", "http://localhost:44334/Home/Login", ""),
                new HttpResponse(new StringWriter())
            );
            var tempData = new TempDataDictionary();
            UsersController controller = new UsersController()
            {
                TempData = tempData
            };
            
            RedirectToRouteResult result = controller.Create(null) as RedirectToRouteResult;
            System.Web.Routing.RouteValueDictionary dictionary = new System.Web.Routing.RouteValueDictionary();
            dictionary.Add("action", "../Home/Index");
            RedirectToRouteResult expected = new RedirectToRouteResult(dictionary);
            Assert.AreEqual(controller.TempData["alertmessage"], "No tiene permisos para acceder a esta página.");
            Assert.AreEqual(result.RouteValues["action"], expected.RouteValues["action"]);
            CurrentUser.deleteCurrentUser("andres@mail.com");
        }

        [TestMethod]
        public void IndexSinPermiso()
        {
            //Nos aseguramos que admin no haya quedado logeado por otros tests.
            CurrentUser.deleteCurrentUser("admin@mail.com");

            CurrentUser.setCurrentUser("andres@mail.com", "Estudiante", "0000000001", "0000000001");
            var httpContext = new HttpContext(
                new HttpRequest("", "http://localhost:44334/Home/Login", ""),
                new HttpResponse(new StringWriter())
            );
            var tempData = new TempDataDictionary();
            UsersController controller = new UsersController()
            {
                TempData = tempData
            };

            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;
            System.Web.Routing.RouteValueDictionary dictionary = new System.Web.Routing.RouteValueDictionary();
            dictionary.Add("action", "../Home/Index");
            RedirectToRouteResult expected = new RedirectToRouteResult(dictionary);
            Assert.AreEqual(controller.TempData["alertmessage"], "No tiene permisos para acceder a esta página");
            Assert.AreEqual(result.RouteValues["action"], expected.RouteValues["action"]);
            CurrentUser.deleteCurrentUser("andres@mail.com");
        }

        [TestMethod]
        public void EditSinPermiso()
        {
            //Arrange
            TestSetup();
            CurrentUser.setCurrentUser("andres@mail.com", "Estudiante", "00000001", "00000001");
            UsersController controller = new UsersController();
            System.Web.Routing.RouteValueDictionary dictionary = new System.Web.Routing.RouteValueDictionary();

            //Act
            RedirectToRouteResult routeResult = controller.Edit("admin@mail", ".com") as RedirectToRouteResult;
            dictionary.Add("action", "../Home/Index");
            RedirectToRouteResult expected = new RedirectToRouteResult(dictionary);

            //Assert
            Assert.AreEqual("No tiene permisos para acceder a esta página.", controller.TempData["alertmessage"]);
            Assert.AreEqual(expected.RouteValues["action"], routeResult.RouteValues["action"]);
            CurrentUser.deleteCurrentUser("andres@mail.com");
        }

        [TestMethod]
        public void EditChangesSaved()
        {
            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());

            var usuarios = new List<Usuario>
            {
                new Usuario() { Activo = true }
            }.AsQueryable();

            var mockDbSetUsuario = new Mock<DbSet<Usuario>>();

            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuarios.Provider);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator());

            var myMockedObjectResult = new Mock<ObjectResult<int>>();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            Persona persona = new Persona();
            persona.Correo = "newusertest@mail.com";
            persona.Nombre1 = "Test";
            persona.Apellido1 = "Nuevo";
            persona.TipoIdentificacion = "Cédula";
            persona.Identificacion = "120540712";
            persona.Estudiante = new Estudiante();
            Usuario usuario = new Usuario
            {
                Activo = true
            };
            UsuarioPersona usuarioPersona = new UsuarioPersona { Persona = persona, Usuario = usuario };

            ObjectParameter parameter = new ObjectParameter("result", typeof(bool));

            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);
            database.Setup(m => m.Usuario).Returns(mockDbSetUsuario.Object);
            database.Setup(m => m.ModificarCorreo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ObjectParameter>())).Callback<string, string, ObjectParameter>((a, b, c) =>
            {
                c.Value = false;
            });

            UsersController controller = new UsersController(database.Object);

            var result = controller.Edit(usuarioPersona) as ViewResult;

            Assert.IsNotNull(result);
            CurrentUser.deleteCurrentUser("admin@mail.com");
        }

        [TestMethod]
        public void EditRepeatedEmail()
        {
            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake2@mail.com", Identificacion = "123456782", Apellido1 = "Fake2", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake3@mail.com", Identificacion = "123456783", Apellido1 = "Fake3", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());

            var usuarios = new List<Usuario>
            {
                new Usuario() { Activo = true }
            }.AsQueryable();

            var mockDbSetUsuario = new Mock<DbSet<Usuario>>();

            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuarios.Provider);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator());

            var myMockedObjectResult = new Mock<ObjectResult<int>>();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            Persona persona = new Persona();
            persona.Correo = "fake2@mail.com";
            persona.Nombre1 = "Test";
            persona.Apellido1 = "Nuevo";
            persona.TipoIdentificacion = "Cédula";
            persona.Identificacion = "120540712";
            persona.Estudiante = new Estudiante();
            Usuario usuario = new Usuario
            {
                Activo = true
            };
            UsuarioPersona usuarioPersona = new UsuarioPersona { Persona = persona, Usuario = usuario };

            ObjectParameter parameter = new ObjectParameter("result", typeof(bool));

            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);
            database.Setup(m => m.Usuario).Returns(mockDbSetUsuario.Object);
            database.Setup(m => m.ModificarCorreo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ObjectParameter>())).Callback<string, string, ObjectParameter>((a, b, c) =>
            {
                c.Value = false;
            });

            UsersController controller = new UsersController(database.Object);

            var firstEdit = controller.Edit("fake1@mail.com", "@mail.com");
            
            var result = controller.Edit(usuarioPersona) as ViewResult;

            Assert.IsNotNull(result);
            CurrentUser.deleteCurrentUser("admin@mail.com");
        }

        [TestMethod]
        public void EditChangesSavedWrongCed()
        {
            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());

            var usuarios = new List<Usuario>
            {
                new Usuario() { Activo = true }
            }.AsQueryable();

            var mockDbSetUsuario = new Mock<DbSet<Usuario>>();

            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuarios.Provider);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator());

            var myMockedObjectResult = new Mock<ObjectResult<int>>();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            Persona persona = new Persona();
            persona.Correo = "newusertest@mail.com";
            persona.Nombre1 = "Test";
            persona.Apellido1 = "Nuevo";
            persona.TipoIdentificacion = "Cédula";
            persona.Identificacion = "120540711232";
            persona.Estudiante = new Estudiante();
            Usuario usuario = new Usuario
            {
                Activo = true
            };
            UsuarioPersona usuarioPersona = new UsuarioPersona { Persona = persona, Usuario = usuario };

            ObjectParameter parameter = new ObjectParameter("result", typeof(bool));

            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);
            database.Setup(m => m.Usuario).Returns(mockDbSetUsuario.Object);
            database.Setup(m => m.ModificarCorreo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ObjectParameter>())).Callback<string, string, ObjectParameter>((a, b, c) =>
            {
                c.Value = false;
            });

            UsersController controller = new UsersController(database.Object);

            var result = controller.Edit(usuarioPersona) as ViewResult;

            Assert.IsNotNull(result);
            CurrentUser.deleteCurrentUser("admin@mail.com");
        }

        [TestMethod]
        public void EditChangesSavedWrongPassport()
        {
            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781231", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());

            var usuarios = new List<Usuario>
            {
                new Usuario() { Activo = true }
            }.AsQueryable();

            var mockDbSetUsuario = new Mock<DbSet<Usuario>>();

            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuarios.Provider);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator());

            var myMockedObjectResult = new Mock<ObjectResult<int>>();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            Persona persona = new Persona();
            persona.Correo = "newusertest@mail.com";
            persona.Nombre1 = "Test";
            persona.Apellido1 = "Nuevo";
            persona.TipoIdentificacion = "Pasaporte";
            persona.Identificacion = "120540712443";
            persona.Estudiante = new Estudiante();
            Usuario usuario = new Usuario
            {
                Activo = true
            };
            UsuarioPersona usuarioPersona = new UsuarioPersona { Persona = persona, Usuario = usuario };

            ObjectParameter parameter = new ObjectParameter("result", typeof(bool));

            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);
            database.Setup(m => m.Usuario).Returns(mockDbSetUsuario.Object);
            database.Setup(m => m.ModificarCorreo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ObjectParameter>())).Callback<string, string, ObjectParameter>((a, b, c) =>
            {
                c.Value = false;
            });

            UsersController controller = new UsersController(database.Object);

            var result = controller.Edit(usuarioPersona) as ViewResult;

            Assert.IsNotNull(result);
            CurrentUser.deleteCurrentUser("admin@mail.com");
        }

        [TestMethod]
        public void EditChangesSavedWrongResidence()
        {
            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781231", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());

            var usuarios = new List<Usuario>
            {
                new Usuario() { Activo = true }
            }.AsQueryable();

            var mockDbSetUsuario = new Mock<DbSet<Usuario>>();

            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuarios.Provider);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator());

            var myMockedObjectResult = new Mock<ObjectResult<int>>();
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            Persona persona = new Persona();
            persona.Correo = "newusertest@mail.com";
            persona.Nombre1 = "Test";
            persona.Apellido1 = "Nuevo";
            persona.TipoIdentificacion = "Residencia";
            persona.Identificacion = "1205407";
            persona.Estudiante = new Estudiante();
            Usuario usuario = new Usuario
            {
                Activo = true
            };
            UsuarioPersona usuarioPersona = new UsuarioPersona { Persona = persona, Usuario = usuario };

            ObjectParameter parameter = new ObjectParameter("result", typeof(bool));

            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);
            database.Setup(m => m.Usuario).Returns(mockDbSetUsuario.Object);
            database.Setup(m => m.ModificarCorreo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ObjectParameter>())).Callback<string, string, ObjectParameter>((a, b, c) =>
            {
                c.Value = false;
            });

            UsersController controller = new UsersController(database.Object);

            var result = controller.Edit(usuarioPersona) as ViewResult;

            Assert.IsNotNull(result);
            CurrentUser.deleteCurrentUser("admin@mail.com");
        }

        [TestMethod]
        public void EditChangesSavedWrongCarne()
        {
            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781231", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());

            var usuarios = new List<Usuario>
            {
                new Usuario() { Activo = true }
            }.AsQueryable();

            var mockDbSetUsuario = new Mock<DbSet<Usuario>>();

            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Provider).Returns(usuarios.Provider);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.Expression).Returns(usuarios.Expression);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.ElementType).Returns(usuarios.ElementType);
            mockDbSetUsuario.As<IQueryable<Usuario>>().Setup(m => m.GetEnumerator()).Returns(usuarios.GetEnumerator());

            var myMockedObjectResult = new Mock<ObjectResult<int>>();

            Persona persona = new Persona();
            persona.Correo = "newusertest@mail.com";
            persona.Nombre1 = "Test";
            persona.Apellido1 = "Nuevo";
            persona.TipoIdentificacion = "Cédula";
            persona.Identificacion = "120540707";
            persona.Estudiante = new Estudiante();
            persona.Estudiante.Carne = "B1234";
            Usuario usuario = new Usuario
            {
                Activo = true
            };
            UsuarioPersona usuarioPersona = new UsuarioPersona { Persona = persona, Usuario = usuario };

            ObjectParameter parameter = new ObjectParameter("result", typeof(bool));

            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);
            database.Setup(m => m.Usuario).Returns(mockDbSetUsuario.Object);
            database.Setup(m => m.ModificarCorreo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ObjectParameter>())).Callback<string, string, ObjectParameter>((a, b, c) =>
            {
                c.Value = false;
            });
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");
            UsersController controller = new UsersController(database.Object);

            var result = controller.Edit(usuarioPersona) as ViewResult;

            Assert.IsNotNull(result);
            CurrentUser.deleteCurrentUser("admin@mail.com");
        }

        [TestMethod]
        public void DeleteTest()
        {
            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake2@mail.com", Identificacion = "123456782", Apellido1 = "Fake2", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake3@mail.com", Identificacion = "123456783", Apellido1 = "Fake3", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());
            
            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);

            UsersController controller = new UsersController(database.Object);

            var result = controller.DeleteConfirmed("fake1", "@mail.com", true) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void WrongDeleteTest()
        {
            var personas = new List<Persona>
            {
                new Persona() { Correo = "fake1@mail.com", Identificacion = "123456781", Apellido1 = "Fake1", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake2@mail.com", Identificacion = "123456782", Apellido1 = "Fake2", Nombre1 = "Fake", TipoIdentificacion = "Cédula" },
                new Persona() { Correo = "fake3@mail.com", Identificacion = "123456783", Apellido1 = "Fake3", Nombre1 = "Fake", TipoIdentificacion = "Cédula" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Persona>>();

            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Provider).Returns(personas.Provider);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.Expression).Returns(personas.Expression);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.ElementType).Returns(personas.ElementType);
            mockDbSet.As<IQueryable<Persona>>().Setup(m => m.GetEnumerator()).Returns(personas.GetEnumerator());

            var database = new Mock<DataIntegradorEntities>();
            database.Setup(m => m.Persona).Returns(mockDbSet.Object);

            UsersController controller = new UsersController(database.Object);

            var result = controller.DeleteConfirmed("fake4", "@mail.com", true) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestInitialize]
        public void TestSetup()
        {
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
        }

        [TestCleanup]
        public void Cleanup()
        {
            CurrentUser.deleteCurrentUser("admin@mail.com");
            CurrentUser.deleteCurrentUser("tamales@mail.com");
        }

    }
}
