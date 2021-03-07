using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppIntegrador.Controllers;
using System.Web.Mvc;
using System.Collections.Generic;
using AppIntegrador.Models;
using System.Linq;
using System.Data.Entity;
using Moq;

namespace AppIntegrador.Tests.Controllers
{
    //Prueba de unidad Lab5 - AndresNavarreteBoza
    [TestClass]
    public class PlanDeMejoraControllerTest
    {
        //Prueba de unidad Lab5 - AndresNavarreteBoza
        [TestMethod]
        public void TestCreateNotNull()
        {
            //Arange
            PlanDeMejoraController plan = new PlanDeMejoraController();

            //Act
            var result = plan.Create() as ViewResult;

            //Assert
            Assert.AreEqual("_crearPlanDeMejora", result.ViewName);
        }

        // Prueba de integracion Lab5 - AndresNavarreteBoza
        [TestMethod]
        public void TestDetallesIntegracion() {
            //Arrange
            var mockDb = new Mock<DataIntegradorEntities>();
            int codigo = 1;

            PlanDeMejora plan = new PlanDeMejora() { codigo = 1, nombre = "planUno" };

            mockDb.Setup(m => m.PlanDeMejora.Find(codigo)).Returns(plan);

            PlanDeMejoraController controller = new PlanDeMejoraController(mockDb.Object);
            // Act 
            ViewResult result = controller.Detalles(codigo) as ViewResult;
            
            // Assert 
            Assert.AreEqual(result.Model, plan);
        }
    }
}
