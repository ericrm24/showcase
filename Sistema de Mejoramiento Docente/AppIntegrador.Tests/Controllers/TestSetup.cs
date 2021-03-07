using AppIntegrador.Controllers;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AppIntegrador.Tests.Controllers
{
    class TestSetup
    {
        class TestableObjectResult<T> : ObjectResult<T>
        {

        }

        /**
         * Método genérico para preparar el Mock del retorno de un procedimiento almacenado.
         * Para más información de cómo funciona, ver
         * https://gisdevblog.wordpress.com/2018/04/04/mocking-stored-procedure-call-in-entity-framework/
         */
        public Mock<ObjectResult<T>> SetupMockProcedure<T>(List<T> data)
        {
            var mockedObjectResult = new Mock<ObjectResult<T>>();
            mockedObjectResult.Setup(x => x.GetEnumerator()).Returns(data.GetEnumerator());
            return mockedObjectResult;
        }

        public void SetupHttpContext(Controller controller)
        {
            if (controller != null)
            {
                controller.ControllerContext = new ControllerContext
                {
                    Controller = controller,
                    HttpContext = new MockHttpContext(new CustomPrincipal("admin@mail.com"))
                };
            }
        }

        public void SetupHttpContextPaco(Controller controller)
        {
            if (controller != null)
            {
                controller.ControllerContext = new ControllerContext
                {
                    Controller = controller,
                    HttpContext = new MockHttpContext(new CustomPrincipal("paco@mail.com"))
                };
            }
        }


        public class CustomPrincipal : IPrincipal
        {
            public IIdentity Identity { get; private set; }
            public bool IsInRole(string role) { return false; }
            public CustomPrincipal(string user)
            {
                Identity = new GenericIdentity(user);
            }
        }

        public class MockHttpContext : HttpContextBase
        {
            private readonly IPrincipal user;

            public MockHttpContext(IPrincipal principal)
            {
                this.user = principal;
            }

            public override IPrincipal User
            {
                get
                {
                    return user;
                }
                set
                {
                    base.User = value;
                }
            }
        }
    }
}
