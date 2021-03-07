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
    // Prueba de unidad que verifica que no se devuelve una vista vacía al intentar crear una pregunta con opciones
    [TestClass]
    public class PreguntaConOpcionesDeSeleccionControllerTest
    {
        [TestMethod]
        public void TestPreguntaOpcionesCreateNotNull()
        {
            PreguntaConOpcionesDeSeleccionController controller = new PreguntaConOpcionesDeSeleccionController();
            ViewResult result = controller.Create() as ViewResult;
            Assert.IsNotNull(result);
        }

        /*[TestMethod]
        public void TestPreguntaOpcionesCreateView()
        {
            PreguntaConOpcionesDeSeleccionController controller = new PreguntaConOpcionesDeSeleccionController();
            ViewResult result = controller.Create() as ViewResult;
            Assert.AreEqual("OpcionesSeleccion", result.ViewName);
        }*/
    }
}