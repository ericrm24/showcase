using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppIntegrador.Controllers;
using AppIntegrador.Models;
using Security.Authentication;
using Moq;
using System.Web;
using System.Web.SessionState;
using System.Reflection;
using System.Security.Principal;
using System.IO;
using System.Web.Routing;

namespace AppIntegrador.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {

        /* TAM-1.1.6 Redirección Login */
        [TestMethod]
        public void LoginView()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Login() as ViewResult;
            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PasswordResetView()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.PasswordReset() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoginAdminSuccess()
        {
            // Arrange
            var formsAuthMock = new Mock<IAuth>();
            HomeController controller = new HomeController(formsAuthMock.Object);

            Usuario usuario = new Usuario
            {
                Username = "admin@mail.com",
                Password = "admin@mail.com",
                Activo = true
            };

            var result = controller.Login(usuario) as RedirectToRouteResult;
            //var logoutresult = controller.Logout() as RedirectToRouteResult;


            //Assert
            Assert.AreEqual("Index", result.RouteValues["action"]);

        }
        /*Termina TAM-1.1.6 Redirección Login*/
                          
        HomeController HomeControllerAs(string userName, bool authenticated)
        {

            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns(userName);
            if (authenticated)
            {
                mock.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            }
            else 
            {
                mock.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(false);
            }

            var controller = new HomeController
            {
                ControllerContext = mock.Object
            };
            return controller;
        }

        [TestMethod]
        public void HomeReestablecerContrasenna()
        {
            //Arrange
            var adminContrasennaActual = "admin@mail.com";
            var adminCorreoActual = "admin@mail.com";
            var pruebaContrasenna = "nuevaContrasenna";

            DataIntegradorEntities db = new DataIntegradorEntities();
            EnlaceSeguroController enlaceController = new EnlaceSeguroController(db);
            var controller = HomeControllerAs("admin@mail.com", false);
            //CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            string url1 = enlaceController.ObtenerEnlaceSeguroAnonimo("/Home/ReestablecerContrasenna/",usuario: adminCorreoActual, reestablecerContrasenna: true, usos: 2);

            // Obtener el hash solamente, deshacerse de /EnlaceSeguro/RedireccionSegura?urlHash=
            string hashObtenido = url1.Substring(url1.LastIndexOf("=") + 1);

            var result = controller.ReestablecerContrasenna(hashObtenido,adminContrasennaActual, adminContrasennaActual, false) as ViewResult;

            // Eliminar las tuplas insertadas
            EnlaceSeguro es1 = db.EnlaceSeguro.Find(hashObtenido);
            if (es1 != null)
            {
                db.EnlaceSeguro.Remove(es1);
                db.SaveChanges();
            }

            var resultadoObtenido = result.ViewBag.NotifyTitle;
            var resultadoEsperado = "Contraseña Cambiada";

            Assert.AreEqual(resultadoObtenido, resultadoEsperado);
        }

        [TestMethod]
        public void HomePasswordReset()
        {
            //Arrange
            var controller = HomeControllerAs("paco@mail.com", false);


            var result = controller.PasswordReset("paco@mail.com", false) as RedirectToRouteResult;

            Assert.AreEqual("Login", result.RouteValues["action"]);
            
        }

        [TestMethod]
        public void HomeIndexResultName()
        {
            // Arrange
            var controller = HomeControllerAs("admin@mail.com", true);
            var noAuthcontroller = HomeControllerAs("admin@mail.com", false);
            controller.TempData["alertmessage"] = "warning test";
            controller.TempData["sweetalertmessage"] = "warning test";

            // Act
            var result = controller.Index() as ViewResult;
            var noAuthResult = noAuthcontroller.Index() as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Index", result.ViewName);
            Assert.AreEqual("Login", noAuthResult.RouteValues["action"]);
        }

        [TestMethod]
        public void HomeAboutResultName()
        {
            // Arrange
            var controller = HomeControllerAs("admin@mail.com", true);
            var noAuthcontroller = HomeControllerAs("admin@mail.com", false);

            // Act
            var result = controller.About() as ViewResult;
            var noAuthResult = noAuthcontroller.Index() as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("About", result.ViewName);
            Assert.AreEqual("Login", noAuthResult.RouteValues["action"]);
        }

        [TestMethod]
        public void HomeCambiarContrasennaResultName()
        {
            // Arrange
            var controller = HomeControllerAs("admin@mail.com", true);
            var noAuthcontroller = HomeControllerAs("admin@mail.com", false);

            // Act
            var result = controller.CambiarContrasenna() as ViewResult;
            var noAuthResult = noAuthcontroller.Index() as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("CambiarContrasenna", result.ViewName);
            Assert.AreEqual("Login", noAuthResult.RouteValues["action"]);
        }

        [TestMethod]
        public void HomeReestablecerContrasennaResultName()
        {
            // Arrange
            var controller = HomeControllerAs("admin@mail.com", true);

            // Act
            var result = controller.ReestablecerContrasenna("placeholder") as ViewResult;

            // Assert
            Assert.AreEqual("ReestablecerContrasenna", result.ViewName);
        }


        [TestMethod]
        public void HomeContactResultName()
        {
            // Arrange
            var controller = HomeControllerAs("admin@mail.com", true);
            var noAuthcontroller = HomeControllerAs("admin@mail.com", false);

            // Act
            var result = controller.Contact() as ViewResult;
            var noAuthResult = noAuthcontroller.Index() as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Contact", result.ViewName);
            Assert.AreEqual("Login", noAuthResult.RouteValues["action"]);
        }

        [TestMethod]
        public void LoginAdminFail()
        {
            // Arrange
            var formsAuthMock = new Mock<IAuth>();
            HomeController controller = new HomeController(formsAuthMock.Object);

            Usuario usuario = new Usuario
            {
                Username = "admin@mail.com",
                Password = "wrong",
                Activo = true
            };

            var result = controller.Login(usuario) as ViewResult;
            Assert.AreEqual("Login", result.ViewName);
        }
        /*Termina TAM-1.1.6 Redirección Login*/

        [TestMethod]
        public void LoginBlock()
        {
            // Arrange
            Usuario usuario = new Usuario
            {
                Username = "tina@mail.com",
                Password = "wrong",
                Activo = true
            };

            using (var context = new DataIntegradorEntities())
            {
                var user = context.Usuario.SingleOrDefault(u => u.Username == usuario.Username);
                if (user != null)
                {
                    user.Activo = true;
                    context.SaveChanges();
                }
            }

            var formsAuthMock = new Mock<IAuth>();
            HomeController controller = new HomeController(formsAuthMock.Object);



            //Se indica que el usuario ya ha fallado 2 veces el password
            //HttpContext.Current.Application["test"] = 2;
            //var apstate = new HttpApplicationState("test");
            //var HCresult = (int)HttpContext.Current.Application["test"];
            CurrentUser.setLoginFailures(2);
            controller.Login(usuario);
            var result = controller.Login(usuario) as ViewResult;
            var testres = controller.ModelState["password"].Errors[0].ErrorMessage;
            string expectedresult = "Usuario y/o contraseña incorrectos";

            using (var context = new DataIntegradorEntities())
            {
                var user = context.Usuario.SingleOrDefault(u => u.Username == usuario.Username);
                if (user != null)
                {
                    user.Activo = true;
                    context.SaveChanges();
                }
            }

            Assert.AreEqual(expectedresult, testres);

        }


        [TestInitialize]
        public void Init()
        {
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
            //SessionStateUtility.AddHttpSessionStateToContext(httpContext, sessionContainer);

        }

        [TestCleanup]
        public void Cleanup()
        {
            //Nos aseguramos que admin quede deslogeado despues de cada test.
            CurrentUser.deleteCurrentUser("admin@mail.com");
        }
    }
}


