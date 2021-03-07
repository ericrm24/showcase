using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppIntegrador;
using AppIntegrador.Controllers;

namespace AppIntegrador.Tests.Controllers
{
    [TestClass]
    public class FormulariosControllerTest
    {
        [TestMethod]
        public void TestFillFormNotNull()
        {
            FormulariosController controller = new FormulariosController();
            ViewResult result = controller.LlenarFormulario("00000001") as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestFillFormView()
        {
            var controller = new FormulariosController();
            var result = controller.LlenarFormulario("00000001") as ViewResult;
            Assert.AreEqual("LlenarFormulario" + '/' + "00000001", result.ViewName);
        }
    }
}
