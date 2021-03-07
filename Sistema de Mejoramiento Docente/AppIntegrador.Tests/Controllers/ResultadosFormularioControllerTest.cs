using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AppIntegrador.Controllers;
using AppIntegrador.Models;
using System.Web.Mvc;
using System.Data.Entity;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AppIntegrador.Tests.Controllers
{
    [TestClass]
    
    public class ResultadosFormularioControllerTest
    {

        //Este test tiene como propósito corroborar que la lista de preguntas que retorna el controlador con base en la consulta es la correcta
        [TestMethod]
        public void TestObtenerPreguntas()
        {

            //Arrange

            //Se crea el mock de la base de datos
            var mockDb = new Mock<DataIntegradorEntities>();

            //Se instancia el controlador y se le pasa como parámetro el mock
            ResultadosFormularioController controller = new ResultadosFormularioController(mockDb.Object);

            //Se crea una lista de preguntas como dummy
            var listaPreguntasDummy = new List<Preguntas>
            {
                new Preguntas { codigoPregunta = "01", codigoSeccion = "01", textoPregunta = "Primera pregunta", tipoPregunta = "U" },
                new Preguntas { codigoPregunta = "02", codigoSeccion = "01", textoPregunta = "Segunda pregunta", tipoPregunta = "M" },
                new Preguntas { codigoPregunta = "03", codigoSeccion = "01", textoPregunta = "Tercera pregunta", tipoPregunta = "L" },
                new Preguntas { codigoPregunta = "04", codigoSeccion = "01", textoPregunta = "Cuarta pregunta", tipoPregunta = "S" },
                new Preguntas { codigoPregunta = "05", codigoSeccion = "01", textoPregunta = "Quinta pregunta", tipoPregunta = "E" }
            };

            //Se crea una lista como dummy de la tabla Formulario
            var formulariosDummy = new List<Formulario> { new Formulario { Codigo = "01", Nombre = "Formulario de prueba" } }.AsQueryable();

            //Se crea una como dummy de la tabla Seccion
            var seccionesDummy = new List<Seccion> { new Seccion { Codigo = "01", Nombre = "Sección de prueba" } }.AsQueryable();

            //Se crea una lista a partir del dummy creado al principio como dummy de la tabla Pregunta
            var preguntasDummy = new List<Pregunta>
            { 
                new Pregunta { Codigo = listaPreguntasDummy[0].codigoPregunta, Enunciado = listaPreguntasDummy[0].textoPregunta, Tipo = listaPreguntasDummy[0].tipoPregunta},
                new Pregunta { Codigo = listaPreguntasDummy[1].codigoPregunta, Enunciado = listaPreguntasDummy[1].textoPregunta, Tipo = listaPreguntasDummy[1].tipoPregunta},
                new Pregunta { Codigo = listaPreguntasDummy[2].codigoPregunta, Enunciado = listaPreguntasDummy[2].textoPregunta, Tipo = listaPreguntasDummy[2].tipoPregunta},
                new Pregunta { Codigo = listaPreguntasDummy[3].codigoPregunta, Enunciado = listaPreguntasDummy[3].textoPregunta, Tipo = listaPreguntasDummy[3].tipoPregunta},
                new Pregunta { Codigo = listaPreguntasDummy[4].codigoPregunta, Enunciado = listaPreguntasDummy[4].textoPregunta, Tipo = listaPreguntasDummy[4].tipoPregunta}
            }.AsQueryable();

            //Se crea una lista como dummy de la tabla Formulario_tiene_seccion
            var formularioTieneSeccionDummy = new List<Formulario_tiene_seccion> { new Formulario_tiene_seccion { FCodigo = "01", SCodigo = "01", Orden = 0 } }.AsQueryable();

            //Se crea una lista como dummy de la tabla Seccion_tiene_pregunta
            var seccionTienePreguntaDummy = new List<Seccion_tiene_pregunta>
            { 
                new Seccion_tiene_pregunta { SCodigo = listaPreguntasDummy[0].codigoSeccion, PCodigo = listaPreguntasDummy[0].codigoPregunta, Orden = 0 },
                new Seccion_tiene_pregunta { SCodigo = listaPreguntasDummy[1].codigoSeccion, PCodigo = listaPreguntasDummy[1].codigoPregunta, Orden = 1 },
                new Seccion_tiene_pregunta { SCodigo = listaPreguntasDummy[2].codigoSeccion, PCodigo = listaPreguntasDummy[2].codigoPregunta, Orden = 2 },
                new Seccion_tiene_pregunta { SCodigo = listaPreguntasDummy[3].codigoSeccion, PCodigo = listaPreguntasDummy[3].codigoPregunta, Orden = 3 },
                new Seccion_tiene_pregunta { SCodigo = listaPreguntasDummy[4].codigoSeccion, PCodigo = listaPreguntasDummy[4].codigoPregunta, Orden = 4 }
            }.AsQueryable();

            //Se hace mock de los modelos de las tablas
            var mockFormularios = new Mock<DbSet<Formulario>>();
            var mockSecciones = new Mock<DbSet<Seccion>>();
            var mockPreguntas = new Mock<DbSet<Pregunta>>();
            var mockFormularioTieneSeccion = new Mock<DbSet<Formulario_tiene_seccion>>();
            var mockSeccionTienePregunta = new Mock<DbSet<Seccion_tiene_pregunta>>();

            //Se settean las propiedades del mock
            this.SetProperties(ref mockFormularios, ref formulariosDummy);
            this.SetProperties(ref mockSecciones, ref seccionesDummy);
            this.SetProperties(ref mockPreguntas, ref preguntasDummy);
            this.SetProperties(ref mockFormularioTieneSeccion, ref formularioTieneSeccionDummy);
            this.SetProperties(ref mockSeccionTienePregunta, ref seccionTienePreguntaDummy);

            //Se hace setup del mock de la base de datos
            mockDb.Setup(x => x.Formulario).Returns(mockFormularios.Object);
            mockDb.Setup(x => x.Seccion).Returns(mockSecciones.Object);
            mockDb.Setup(x => x.Pregunta).Returns(mockPreguntas.Object);
            mockDb.Setup(x => x.Formulario_tiene_seccion).Returns(mockFormularioTieneSeccion.Object);
            mockDb.Setup(x => x.Seccion_tiene_pregunta).Returns(mockSeccionTienePregunta.Object);

            //Act

            //Se hace el llamado al controller
            var listaPreguntasController = controller.ObtenerPreguntas("01");

            //Se inicia una comparación para verificar si la lista de preguntas retornada por el controller es igual al dummy

            //Se verifica si la cantidad de preguntas es la misma
            bool resultado = listaPreguntasDummy.Count() == listaPreguntasController.Count();
            int indice = 0;

            //Mientras las respuestas sean las mismas y no se acabe la lista
            while (resultado && indice < listaPreguntasDummy.Count())
            {
                //Compara cada pregunta
                resultado = listaPreguntasDummy[indice].codigoPregunta.Equals(listaPreguntasController[indice].codigoPregunta)
                            && listaPreguntasDummy[indice].codigoSeccion.Equals(listaPreguntasController[indice].codigoSeccion)
                            && listaPreguntasDummy[indice].textoPregunta.Equals(listaPreguntasController[indice].textoPregunta)
                            && this.SwitchTipo(listaPreguntasDummy[indice].tipoPregunta).Equals(listaPreguntasController[indice].tipoPregunta);
                            
                ++indice;
            }

            //Assert
            Assert.IsTrue(resultado);

        }

        // Este test tiene como propósito verificar que el método que obtiene las opciones seleccionadas 
        // de una pregunta de selección única válida y que se sabe, tiene respuestas, no de null
        [TestMethod]
        public void TestOpcionesSeleccionadasPreguntaSeleccion()
        {
            // Arrange
            ResultadosFormularioController controller = new ResultadosFormularioController();
            //Act 
            string result = controller.ObtenerOpcionesSeleccionadasPreguntasSeleccion("00000001", "CI0128", 1, 2, 2019, DateTime.Parse("31/10/2019"), DateTime.Parse("02/11/2019"), "00000001", "00000001", 3);
            // Assert
            Assert.IsNotNull(result);

        }


        // Este test tiene como propósito verificar que el título de la justificacion de una pregunta que retorna el controller sea la correcta
        [TestMethod]
        public void TestTituloJustificacion()
        {
            // Se crea el mock de la base de datos
            var mockDb = new Mock<DataIntegradorEntities>();

            // Se instancia el controlador y se le pasa el mock
            ResultadosFormularioController controller = new ResultadosFormularioController(mockDb.Object);

            // Se crea una lista de titulos de justificaciones dummys
            var TitulosDummy = new List<Pregunta_con_opciones>
            {
                new Pregunta_con_opciones{ Codigo = "01", TituloCampoObservacion = "Justificacion 1" },
                new Pregunta_con_opciones{ Codigo = "02", TituloCampoObservacion = "Justificacion 2" },
                new Pregunta_con_opciones{ Codigo = "03", TituloCampoObservacion = "Justificacion 3" }
            }.AsQueryable();

            // Se hace el mock de la tabla Pregunta_con_opciones
            var mockPreguntasOpciones = new Mock<DbSet<Pregunta_con_opciones>>();

            // Se "settean" las propiedades del mock
            this.SetProperties(ref mockPreguntasOpciones, ref TitulosDummy);

            //Se hace setup del mock de la base de datos
            mockDb.Setup(x => x.Pregunta_con_opciones).Returns(mockPreguntasOpciones.Object);

            //Se hace el llamado al controler

            var listaTitulosDummy = TitulosDummy.ToList();
            var listaTitulosController = new List<string>();

            for (var i = 0; i < listaTitulosDummy.Count(); ++i)
            {
                listaTitulosController.Add(controller.getTituloJustificacion(listaTitulosDummy[i].Codigo));
            }

            //Se verifica si la cantidad de titulos obtenidos es la misma
            bool resultado = listaTitulosDummy.Count() == listaTitulosController.Count();

            int indice = 0;

            //Mientras los titulos sean los mismos y no se acabe la lista
            while (resultado && indice < listaTitulosDummy.Count())
            {
                //Compara cada titulo de cada justificación
                resultado = (listaTitulosDummy[indice].TituloCampoObservacion).Equals(listaTitulosController[indice]);

                ++indice;
            }

            //Assert
            Assert.IsTrue(resultado);

        }


        //Este test tiene como propósito verificar que el string con el tipo de pregunta que retorna el controller sea el correcto
        [TestMethod]
        public void TestGetTipoPregunta()
        {

            //Arrange

            //Se crea el mock de la base de datos
            var mockDb = new Mock<DataIntegradorEntities>();

            //Se instancia el controlador y se le pasa como parámetro el mock
            ResultadosFormularioController controller = new ResultadosFormularioController(mockDb.Object);

            //Se crea una lista de preguntas como dummy de la tabla Pregunta
            var preguntasDummy = new List<Pregunta>
            {
                new Pregunta { Codigo = "01", Enunciado = "Primera pregunta", Tipo = "M" },
                new Pregunta { Codigo = "02", Enunciado = "Segunda pregunta", Tipo = "L" },
                new Pregunta { Codigo = "03", Enunciado = "Tercera pregunta", Tipo = "U" },
                new Pregunta { Codigo = "04", Enunciado = "Cuarta pregunta", Tipo = "E" },
                new Pregunta { Codigo = "05", Enunciado = "Quinta pregunta", Tipo = "S" }
            }.AsQueryable();

            //Se hace mock de la tabla Pregunta
            var mockPreguntas = new Mock<DbSet<Pregunta>>();

            //Se settean las propiedades del mock
            this.SetProperties(ref mockPreguntas, ref preguntasDummy);

            //Se hace setup del mock de la base de datos
            mockDb.Setup(x => x.Pregunta).Returns(mockPreguntas.Object);

            //Act

            //Se hace el llamado al controler

            var listaPreguntasDummy = preguntasDummy.ToList();
            var listaTiposController = new List<string>();
            for (var i = 0; i < listaPreguntasDummy.Count(); ++i)
            {
                listaTiposController.Add(controller.GetTipoPregunta(listaPreguntasDummy[i].Codigo));
            }

            //Se verifica si la cantidad de preguntas es la misma
            bool resultado = listaPreguntasDummy.Count() == listaTiposController.Count();
            int indice = 0;

            //Mientras las respuestas sean las mismas y no se acabe la lista
            while (resultado && indice < listaPreguntasDummy.Count())
            {
                //Compara cada tipo de pregunta
                resultado = this.SwitchTipo(listaPreguntasDummy[indice].Tipo).Equals(listaTiposController[indice]);

                ++indice;
            }

            //Assert
            Assert.IsTrue(resultado);

        }

        //COD-4: Visualizar el promedio para las respuestas de las preguntas de tipo escalar
        [TestMethod]
        public void TestPromedioFormulario() //Comprueba que el promedio retornado no es nulo
        {
            ResultadosFormularioController controller = new ResultadosFormularioController();
            String resultado = controller.getPromedio("00000001", "CI0128", 1, 2, 2019, DateTime.Parse("31/10/2019"), DateTime.Parse("02/11/2019"), "00000001", "00000001") as String; //Metodo en el controlador y sus respectivos parametros
            Assert.IsNotNull(resultado); //Comprobacion de que no es null 
        }

        // COD-22: Visualizar la desviación estándar para las respuestas de las preguntas de escala numérica
        [TestMethod]
        public void TestDesviacionFormulario()
        {
            // Arrange
            ResultadosFormularioController controlador = new ResultadosFormularioController();
            // Act
            String resultado = controlador.obtenerDesviacionEstandar("00000001", "CI0128", 1, 2, 2019, DateTime.Parse("31/10/2019"), DateTime.Parse("02/11/2019"), "00000001", "00000001") as String; //Metodo en el controlador y sus respectivos parametros
            // Assert
            Assert.IsNotNull(resultado); //Comprobacion de que no es null 
        }

        [TestMethod]
        public void TestJustificacionNotNULL()
        {
            // Arrange
            ResultadosFormularioController controller = new ResultadosFormularioController();
            // Act
            String result = controller.getJustificacionPregunta("00000001", "CI0128", 1, 2, 2019, DateTime.Parse("31/10/2019"), DateTime.Parse("02/11/2019"), "00000001", "00000001");
            // Assert
            Assert.IsNotNull(result);
        }

        /*Este test tiene como propósito probar lo que retorna el método para obtener las justificaciones de una pregunta cuando
        se envían parámetros inválidos.*/
        [TestMethod]
        public void TestJustificacionParametrosInvalidos()
        {

            // Arrange
            ResultadosFormularioController controller = new ResultadosFormularioController();

            // Act
            string justificacionesJson = controller.getJustificacionPregunta("0A500W01", "CI0128", 1, 2, 2019, DateTime.Parse("01/03/2019"), DateTime.Parse("02/03/2019"), null, "00000001");
            var justificaciones = JsonConvert.DeserializeObject<List<SelectListItem>>(justificacionesJson);

            // Assert
            Assert.IsTrue(justificaciones.Count() == 0);

        }

        [TestMethod]
        //Este test tiene como propósito corroborar que las respuestas de texto abierto que retorna el controlador se serializan correctamente.
        public void TestObtenerRespuestasTextoAbierto()
        {

            //Arrange

            //Se crea el mock de la base de datos
            var mockDb = new Mock<DataIntegradorEntities>();

            //Se instancia el controlador y se le pasa como parámetro el mock
            ResultadosFormularioController controller = new ResultadosFormularioController(mockDb.Object);

            //Se crea una lista de respuestas como dummy data

            var respuestasFormularioDummy = new List<Respuestas_a_formulario>
            {
                new Respuestas_a_formulario() { FCodigo = "01", Correo = "admin1@mail.com", CSigla = "CI0128", GNumero = 1, GSemestre = 2, GAnno = 2019, Fecha = DateTime.Parse("01/11/2019"), Finalizado = true },
                new Respuestas_a_formulario() { FCodigo = "01", Correo = "admin2@mail.com", CSigla = "CI0128", GNumero = 1, GSemestre = 2, GAnno = 2019, Fecha = DateTime.Parse("01/11/2019"), Finalizado = true },
                new Respuestas_a_formulario() { FCodigo = "01", Correo = "admin3@mail.com", CSigla = "CI0128", GNumero = 1, GSemestre = 2, GAnno = 2019, Fecha = DateTime.Parse("01/11/2019"), Finalizado = true },
                new Respuestas_a_formulario() { FCodigo = "01", Correo = "admin4@mail.com", CSigla = "CI0128", GNumero = 1, GSemestre = 2, GAnno = 2019, Fecha = DateTime.Parse("01/11/2019"), Finalizado = true },
                new Respuestas_a_formulario() { FCodigo = "01", Correo = "admin5@mail.com", CSigla = "CI0128", GNumero = 1, GSemestre = 2, GAnno = 2019, Fecha = DateTime.Parse("01/11/2019"), Finalizado = true }
            }.AsQueryable();

            var respuestasDummy = new List<Responde_respuesta_libre>
            {
                
                new Responde_respuesta_libre() { FCodigo = "01", Correo = "admin1@mail.com", CSigla = "CI0128", GNumero = 1, GSemestre = 2, GAnno = 2019, Fecha = DateTime.Parse("01/11/2019"), PCodigo = "01", SCodigo = "01", Observacion = "Respuesta1" },
                new Responde_respuesta_libre() { FCodigo = "01", Correo = "admin2@mail.com", CSigla = "CI0128", GNumero = 1, GSemestre = 2, GAnno = 2019, Fecha = DateTime.Parse("01/11/2019"), PCodigo = "01", SCodigo = "01", Observacion = "Respuesta1" },
                new Responde_respuesta_libre() { FCodigo = "01", Correo = "admin3@mail.com", CSigla = "CI0128", GNumero = 1, GSemestre = 2, GAnno = 2019, Fecha = DateTime.Parse("01/11/2019"), PCodigo = "01", SCodigo = "01", Observacion = "Respuesta1" },
                new Responde_respuesta_libre() { FCodigo = "01", Correo = "admin4@mail.com", CSigla = "CI0128", GNumero = 1, GSemestre = 2, GAnno = 2019, Fecha = DateTime.Parse("01/11/2019"), PCodigo = "01", SCodigo = "01", Observacion = "Respuesta1" },
                new Responde_respuesta_libre() { FCodigo = "01", Correo = "admin5@mail.com", CSigla = "CI0128", GNumero = 1, GSemestre = 2, GAnno = 2019, Fecha = DateTime.Parse("01/11/2019"), PCodigo = "01", SCodigo = "01", Observacion = "Respuesta1" }

            }.AsQueryable();

            //Se hace mock del modelo de la tabla
            var mockDbSetRespuestasFormulario = new Mock<DbSet<Respuestas_a_formulario>>();
            var mockDbSet = new Mock<DbSet<Responde_respuesta_libre>>();
            this.SetProperties(ref mockDbSetRespuestasFormulario, ref respuestasFormularioDummy);
            this.SetProperties(ref mockDbSet, ref respuestasDummy);

            //Se hace setup del mock de la base de datos
            mockDb.Setup(x => x.Respuestas_a_formulario).Returns(mockDbSetRespuestasFormulario.Object);
            mockDb.Setup(x => x.Responde_respuesta_libre).Returns(mockDbSet.Object);

            //Act

            //Se convierte el querable de respuestas dummy a una lista para compararla más fácilmente con lo que retorna el controller
            var listaRespuestasDummy = respuestasDummy.ToList();

            //Se hace el llamado al controller
            string respuestasJson = controller.ObtenerRespuestasTextoAbierto("01", "CI0128", 1, 2, 2019, DateTime.Parse("01/11/2019"), DateTime.Parse("02/11/2019"), "01", "01");

            //Se deserializa el JSON que retorna el controller
            var respuestasController = JsonConvert.DeserializeObject<List<SelectListItem>>(respuestasJson);

            //Se inicia una comparación para verificar si las respuestas de texto abierto retornadas por el controller son iguales a las del mock.

            //Se verifica si la cantidad de respuestas distintas es la misma
            bool resultado = respuestasDummy.Count() == respuestasController.Count();
            int indice = 0;

            //Mientras las respuestas sean las mismas y no se acabe la lista
            while (resultado && indice < respuestasDummy.Count())
            {
                //Compara cada respuesta
                resultado = listaRespuestasDummy[indice].Observacion.Equals(respuestasController[indice].Value);
                ++indice;
            }

            //Assert
            Assert.IsTrue(resultado);

        }

        //Settea las propiedades del mock
        private void SetProperties<T>(ref Mock<DbSet<T>> mock, ref IQueryable<T> dummy) where T : class
        {
            mock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(dummy.Provider);
            mock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(dummy.Expression);
            mock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(dummy.ElementType);
            mock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(dummy.GetEnumerator());
        }

        private string SwitchTipo(string tipoTabla)
        {
            string tipoScript = null;

            switch (tipoTabla)
            {
                case "U": tipoScript = "seleccion_unica"; break;
                case "M": tipoScript = "seleccion_multiple"; break;
                case "L": tipoScript = "texto_abierto"; break;
                case "S": tipoScript = "seleccion_cerrada"; break;
                case "E": tipoScript = "escala"; break;
            }

            return tipoScript;

        }

        [TestMethod]
        public void TestSeccionesFormulario()
        {
            // Arrange
            ResultadosFormularioController controller = new ResultadosFormularioController();
            //Act 
            IEnumerable<SelectListItem> result = controller.ObtenerSeccionesDropDown("00000001");
            // Assert
            Assert.IsNotNull(result);
        }

    }
}
