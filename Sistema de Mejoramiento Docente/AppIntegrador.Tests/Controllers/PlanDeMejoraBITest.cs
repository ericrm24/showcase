using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AppIntegrador.Controllers.PlanesDeMejoraBI;
using AppIntegrador.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AppIntegrador.Tests.Controllers
{
    [TestClass]
    public class PlanDeMejoraBITest
    {
        // Test de a asignacion de un plan de mejora cuando la base de 
        // datos no esta poblada
        [TestMethod]
        public void TestSetCodigoEmptyDB()
        {
            // Se crea una base de datos vacia
            DataIntegradorEntities test = new DataIntegradorEntities();
            var db = new Mock<DataIntegradorEntities>();

            // Datos del plan de mejora temporal
            String planNombre = "Plan de prueba";
            DateTime inicio = new DateTime(2019, 12, 01);
            DateTime Fin = new DateTime(2020, 12, 01);

            // Crando un plan de mejora temporal
            PlanDeMejora plan = new PlanDeMejora()
            {
                nombre = planNombre,
                fechaInicio = inicio,
                fechaFin = Fin
            };

            // controlador planes de mejora BI
            var controller = new PlanDeMejoraBI();

            // Metodo que agrega el codigo al plan de mejora
            controller.setCodigoAPlanDeMejora(test, plan);

            // Vemos que se le asigne un codigo con sentido al plan de mejora
            controller.setCodigoAPlanDeMejora(test, plan);
            Assert.IsNotNull(plan.codigo);
        }

        //Test del seteo de formulario con plan de mejora
        //Sin Formularios
        [TestMethod]
        public void insertFormulariosTestSinForms()
        {
            PlanDeMejora plan = new PlanDeMejora();
            List<string> codFormularios = new List<string>();
            DataIntegradorEntities db = new DataIntegradorEntities();

            //Seteo de valor nulo
            plan.Formulario = null;

            var controller = new PlanDeMejoraBI();
            controller.insertFormularios(plan, codFormularios, db);

            Boolean result = plan.Formulario.Count > 0 ? true : false;

            Assert.IsFalse(result);
        }

        //Test del seteo de formulario con plan de mejora
        //Con formularios
        [TestMethod]
        public void insertFormulariosTestConForms()
        {
            PlanDeMejora plan = new PlanDeMejora();
            List<string> codFormularios = new List<string>();
            DataIntegradorEntities db = new DataIntegradorEntities();

            //Agregnado elementos
            for (int var = 0; var < 5; var++)
            {
                codFormularios.Add(var + "");
            }

            //Seteo de valor nulo
            plan.Formulario = null;

            var controller = new PlanDeMejoraBI();
            controller.insertFormularios(plan, codFormularios, db);

            Boolean result = plan.Formulario.Count > 0 ? true : false;

            Assert.IsTrue(result);
        }

        //Test del seteo de secciones en objetivos
        //Sin secciones
        [TestMethod]
        public void insertSeccionesInObjetivosEmpty()
        {
            int tamanno = 4;

            Boolean resultado = true;
            // Creacion de los objetivos
            var objetivos = new List<Objetivo>
            {
                new Objetivo() { codPlan = 32, nombre = "00000000" },
                new Objetivo() { codPlan = 32, nombre = "00000001" },
                new Objetivo() { codPlan = 32, nombre = "00000002" },
                new Objetivo() { codPlan = 32, nombre = "00000003" },
            }.AsQueryable();

            var mockObjetivosDBSet = new Mock<DbSet<Objetivo>>();

            mockObjetivosDBSet.As<IQueryable<Objetivo>>().Setup(m => m.Provider).Returns(objetivos.Provider);
            mockObjetivosDBSet.As<IQueryable<Objetivo>>().Setup(m => m.Expression).Returns(objetivos.Expression);
            mockObjetivosDBSet.As<IQueryable<Objetivo>>().Setup(m => m.ElementType).Returns(objetivos.ElementType);
            mockObjetivosDBSet.As<IQueryable<Objetivo>>().Setup(m => m.GetEnumerator()).Returns(objetivos.GetEnumerator());

            // Ahora creando las tablas de las secciones
            var secciones = new List<Seccion>
            {
                new Seccion() { Codigo = "00000000" },
                new Seccion() { Codigo = "00000001" },
                new Seccion() { Codigo = "00000002" },
                new Seccion() { Codigo = "00000003" }
            }.AsQueryable();

            var mockSeccionesDBSet = new Mock<DbSet<Seccion>>();

            mockSeccionesDBSet.As<IQueryable<Seccion>>().Setup(m => m.Provider).Returns(secciones.Provider);
            mockSeccionesDBSet.As<IQueryable<Seccion>>().Setup(m => m.Expression).Returns(secciones.Expression);
            mockSeccionesDBSet.As<IQueryable<Seccion>>().Setup(m => m.ElementType).Returns(secciones.ElementType);
            mockSeccionesDBSet.As<IQueryable<Seccion>>().Setup(m => m.GetEnumerator()).Returns(secciones.GetEnumerator());

            var mockDb = new Mock<DataIntegradorEntities>();

            //Seteo de las tablas
            mockDb.Setup(m => m.Objetivo).Returns(mockObjetivosDBSet.Object);
            mockDb.Setup(m => m.Seccion).Returns(mockSeccionesDBSet.Object);

            PlanDeMejoraBI controller = new PlanDeMejoraBI();

            //Haciendo el diccionario que crea la relacion entre los objetivos y las secciones
            Dictionary<String, String> dic = new Dictionary<string, string>();
            //Agregando una unica seccion por objetivo
            for (int i = 0; i < tamanno; i++)
            {
                var value = "0000000" + i;
                dic.Add(value, value);
            }

            controller.insertSeccionesEnObjetivos(objetivos.ToList(), dic, mockDb.Object);

            var seccionesTemp = mockDb.Object.Seccion.ToList();
            var objetivosTemp = mockDb.Object.Objetivo.ToList();

            //Ahora ya con los objetivos modificados hacemos las asociaciones
            for (int i = 0; i < 4; ++i)
            {

                string identificadorObjetivo = objetivosTemp[i].nombre;
                var identificadorSeccion = objetivosTemp[i].Seccion;

                if (identificadorObjetivo == null)
                {
                    resultado = false;
                }
            }

            //Ahora vemos si pasa la prueba
            Assert.IsTrue(resultado);
        }

        //Test del seteo de Preguntas con acciones de mejora
        //Con secciones
        /*[TestMethod]
        public void insertPreguntasInAccionesDeMejora()
        {
            int tamanno = 4;

            Boolean resultado = true;

            AccionDeMejora acc = new AccionDeMejora();

            var plan = new PlanDeMejora();
            plan.Objetivo = new List<Objetivo>();
            plan.Objetivo.Add( new Objetivo() { codPlan = 35, nombre = "asd", 
            AccionDeMejora = new List<AccionDeMejora>() {
                new AccionDeMejora () { }
            }
            });



            // Creacion de los objetivos
            var plan = new List<PlanDeMejora>
            {
                new AccionDeMejora() { },
                new AccionDeMejora() { codPlan = 32, nombreObj = "asd", descripcion = "00000001" },
                new AccionDeMejora() { codPlan = 32, nombreObj = "asd", descripcion = "00000002" },
                new AccionDeMejora() { codPlan = 32, nombreObj = "asd", descripcion = "00000003" }
            }.AsQueryable();

            var mockAccionesDeMejoraDBSet = new Mock<DbSet<AccionDeMejora>>();

            mockAccionesDeMejoraDBSet.As<IQueryable<AccionDeMejora>>().Setup(m => m.Provider).Returns(acciones.Provider);
            mockAccionesDeMejoraDBSet.As<IQueryable<AccionDeMejora>>().Setup(m => m.Expression).Returns(acciones.Expression);
            mockAccionesDeMejoraDBSet.As<IQueryable<AccionDeMejora>>().Setup(m => m.ElementType).Returns(acciones.ElementType);
            mockAccionesDeMejoraDBSet.As<IQueryable<AccionDeMejora>>().Setup(m => m.GetEnumerator()).Returns(acciones.GetEnumerator());

            // Ahora creando las tablas de las secciones
            var preguntas = new List<Preguntas>
            {
                new Pregunta() { Codigo = "00000000" },
                new Pregunta() { codigoPregunta = "00000001" },
                new Pregunta() { codigoPregunta = "00000002" },
                new Pregunta() { codigoPregunta = "00000003" }
            }.AsQueryable();

            var mockPreguntasDBSet = new Mock<DbSet<Seccion>>();

            mockPreguntasDBSet.As<IQueryable<Pregunta>>().Setup(m => m.Provider).Returns(preguntas.Provider);
            mockPreguntasDBSet.As<IQueryable<Pregunta>>().Setup(m => m.Expression).Returns(preguntas.Expression);
            mockPreguntasDBSet.As<IQueryable<Pregunta>>().Setup(m => m.ElementType).Returns(preguntas.ElementType);
            mockPreguntasDBSet.As<IQueryable<Pregunta>>().Setup(m => m.GetEnumerator()).Returns(preguntas.GetEnumerator());

            var mockDb = new Mock<DataIntegradorEntities>();

            //Seteo de las tablas
            mockDb.Setup(m => m.AccionDeMejora).Returns(mockAccionesDeMejoraDBSet.Object);
            mockDb.Setup(m => m.Pregunta).Returns(mockPreguntasDBSet.Object);

            PlanDeMejoraBI controller = new PlanDeMejoraBI();

            //Haciendo el diccionario que crea la relacion entre los objetivos y las secciones
            Dictionary<String, String> dic = new Dictionary<string, string>();
            //Agregando una unica seccion por objetivo
            for (int i = 0; i < tamanno; i++)
            {
                var value = "0000000" + i;
                dic.Add(value, value);
            }

            controller.insertSeccionesEnObjetivos(acciones.ToList(), dic, mockDb.Object);

            var a = mockDb.Object.Pregunta.ToList();
            var b = mockDb.Object.AccionDeMejora.ToList();

            //Ahora ya con los objetivos modificados hacemos las asociaciones
            for (int i = 0; i < 4; ++i)
            {

                var identificadorA = |;
                var identificadorB = objetivosTemp[i].Seccion;

                if (identificadorObjetivo == null)
                {
                    resultado = false;
                }
            }

            //Ahora vemos si pasa la prueba
            Assert.IsTrue(resultado);
        }*/
    }
}
