using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using AppIntegrador.Controllers;
using AppIntegrador.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppIntegrador.Tests.Controllers
{
    [TestClass]
    public class EnlaceSeguroControllerTest
    {
        [TestMethod]
        public void TestObtenerEnlaceSeguroUnico()
        {
            DataIntegradorEntities db = new DataIntegradorEntities();
            EnlaceSeguroController controller = new EnlaceSeguroController(db);
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            string url1 = controller.ObtenerEnlaceSeguro("https://localhost:44334/Home/About/");
            string url2 = controller.ObtenerEnlaceSeguro("https://localhost:44334/Home/About/");

            // Obtener el hash solamente, deshacerse de /EnlaceSeguro/RedireccionSegura?urlHash=
            string hash1 = url1.Substring(url1.LastIndexOf("=") + 1);
            string hash2 = url2.Substring(url2.LastIndexOf("=") + 1);

            // Eliminar las tuplas insertadas
            EnlaceSeguro es1 = db.EnlaceSeguro.Find(hash1);
          if (es1 != null)
            {
                db.EnlaceSeguro.Remove(es1);
                db.SaveChanges();
            }

            EnlaceSeguro es2 = db.EnlaceSeguro.Find(hash2);
            if (es2 != null)
            {
                db.EnlaceSeguro.Remove(es2);
                db.SaveChanges();
            }

            Assert.AreNotEqual(url1, url2);
        }

        [TestMethod]
        public void TestRedireccionEnlaceSeguroCorrecto()
        {
            DataIntegradorEntities db = new DataIntegradorEntities();
            EnlaceSeguroController controller = new EnlaceSeguroController(db);
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            // Permitir solamente a admin@mail.com
            string url = controller.ObtenerEnlaceSeguro("https://localhost:44334/Home/About/", "admin@mail.com");

            // Obtener el hash solamente, deshacerse de /EnlaceSeguro/RedireccionSegura?urlHash=
            string hash = url.Substring(url.LastIndexOf("=") + 1);

            var result = controller.RedireccionSegura(hash) as RedirectResult;

            // Eliminar las tuplas insertadas
            EnlaceSeguro es = db.EnlaceSeguro.Find(hash);
            if (es != null)
            {
                db.EnlaceSeguro.Remove(es);
                db.SaveChanges();
            }

            Assert.AreEqual("https://localhost:44334/Home/About/", result.Url);
        }

        [TestMethod]
        public void TestRedireccionEnlaceSeguroIncorrecto()
        {
            DataIntegradorEntities db = new DataIntegradorEntities();
            EnlaceSeguroController controller = new EnlaceSeguroController(db);
            CurrentUser.setCurrentUser("admin@mail.com", "Superusuario", "0000000001", "0000000001");

            // Permitir solamente a tamales@mail.com
            string url = controller.ObtenerEnlaceSeguro("/Home/About/", "tamales@mail.com");

            // Obtener el hash solamente, deshacerse de /EnlaceSeguro/RedireccionSegura?urlHash=
            string hash = url.Substring(url.LastIndexOf("=") + 1);

            var result = controller.RedireccionSegura(hash) as RedirectToRouteResult;

            // Eliminar las tuplas insertadas
            EnlaceSeguro es = db.EnlaceSeguro.Find(hash);
            if (es != null)
            {
                db.EnlaceSeguro.Remove(es);
                db.SaveChanges();
            }

            Assert.AreEqual("Index", result.RouteValues["action"]);
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
        }

        [TestCleanup]
        public void Cleanup()
        {
            //Nos aseguramos que admin quede deslogeado despues de cada test.
            CurrentUser.deleteCurrentUser("admin@mail.com");
        }
    }
}
