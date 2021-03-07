using System;
using System.Collections.Generic;
using AppIntegrador.Models;
using AppIntegrador.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppIntegrador.Tests.Utilities
{
    [TestClass]
    public class EmailNotificationTest
    {
        private DataIntegradorEntities db;

        /*[TestMethod]
        public void TestSendNotification()
        {
            EmailNotification notification = new EmailNotification();
            // Por ahora utilizo mi propio correo
            List<string> recipients = new List<string> { "ericrios24@gmail.com" };
            string body = "Este es un correo de prueba";

            int result = notification.SendNotification(recipients, "Correo Test", body, body);

            Assert.AreEqual(result, 0);
        }*/
    }
}
