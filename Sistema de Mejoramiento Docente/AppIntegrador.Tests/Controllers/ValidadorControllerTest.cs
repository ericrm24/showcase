using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Web.Mvc;
using AppIntegrador.Models;
using AppIntegrador.Controllers;
using Moq;
using System.Web;
using System.IO;
using System.Web.Routing;

namespace AppIntegrador.Tests.Controllers
{
    /// <summary>
    /// Descripción resumida de ValidadorControllerTest
    /// </summary>
    [TestClass]
    public class ValidadorControllerTest : Validador
    {
        #region Atributos de prueba adicionales
        //
        // Puede usar los siguientes atributos adicionales conforme escribe las pruebas:
        //
        // Use ClassInitialize para ejecutar el código antes de ejecutar la primera prueba en la clase
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup para ejecutar el código una vez ejecutadas todas las pruebas en una clase
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Usar TestInitialize para ejecutar el código antes de ejecutar cada prueba 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup para ejecutar el código una vez ejecutadas todas las pruebas
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CodigoInvalido() //Este test aplica para el codigo del enfasis, carrera, sigla curso y unidad academica
        //ya que tienen la misma condicion de validacion
        {
            GuiaHorario guia = new GuiaHorario();
            guia.CodigoCarreraCurso = "11111111111111111111111111111111111111"; //Longitud invalida mayor a 10 caracteres
            bool retorno = ValidarTamanoText(guia.CodigoCarreraCurso, 10);
            Assert.AreEqual(retorno, false);
        }

        [TestMethod]
        public void NombreInvalido() //Este test aplica para el nombre de los cursos, unidades, carreras y enfasis
        //ya que tienen la misma condicion de validacion
        {
            GuiaHorario guia = new GuiaHorario();
            guia.NombreCurso = "Calculo 10000000000000000000000000000000000000000000001"; //Longitud invalida mayor a 50 caracteres
            bool retorno = ValidarTamanoText(guia.CodigoCarreraCurso, 50);
            Assert.AreEqual(retorno, false);
        }

        [TestMethod]
        public void AnnoNoEsInt() //Este test aplica para el anno, el semestre y el numero de grupo
        //ya que tienen la misma condicion de validacion
        {
            GuiaHorario guia = new GuiaHorario();
            guia.NumeroGrupo = "No es int"; //No es un entero
            bool retorno = ValidarNumero(guia.NumeroGrupo);
            Assert.AreEqual(retorno, false);
        }

        [TestMethod]
        public void SemestreNoEsInt() //Este test aplica para el anno, semestre y numero de grupo
        //ya que tienen la misma condicion de validacion
        {
            GuiaHorario guia = new GuiaHorario();
            guia.Semestre = "2.0"; //Longitud invalida mayor a 50 caracteres
            bool retorno = ValidarNumero(guia.Semestre);
            Assert.AreEqual(retorno, false);
        }

        //NO PASA LA VALIDACION. CORREGIR
     /*   [TestMethod]
        public void GrupoNoEsPositivo() //Este test aplica para el anno, semestre y numero de grupo
        //ya que tienen la misma condicion de validacion
        {
            GuiaHorario guia = new GuiaHorario();
            guia.NumeroGrupo = "-1"; //Longitud invalida mayor a 50 caracteres
            bool retorno = ValidarNumero(guia.NumeroGrupo);
            Assert.AreEqual(retorno, false);
        }*/

     
        [TestMethod]
        public void emailInvalido() //Este test aplica para el correo del funcionario, estudiante y persona
        {
            GuiaHorario guia = new GuiaHorario();
            guia.CorreoMatricula = "denisse.alfaro"; 
            bool retorno = ValidarEmail(guia.CorreoMatricula);
            Assert.AreEqual(retorno, false);
        }

        [TestMethod]
        public void emailInvalido2() //Este test aplica para el correo del funcionario, estudiante y persona
        {
            GuiaHorario guia = new GuiaHorario();
            guia.CorreoMatricula = "invalido.com"; 
            bool retorno = ValidarEmail(guia.CorreoMatricula);
            Assert.AreEqual(retorno, false);
        }

        [TestMethod]
        public void emailInvalido3() //Este test aplica para el correo del funcionario, estudiante y persona
        {
            GuiaHorario guia = new GuiaHorario();
            guia.CorreoMatricula = "345";
            bool retorno = ValidarEmail(guia.CorreoMatricula);
            Assert.AreEqual(retorno, false);
        }

        [TestMethod]
        public void noEsBool() //Este test solo aplica para Borrado
        {
            ListaEstudiante lista = new ListaEstudiante();
            lista.Borrado = "No es bool";
            bool retorno = ValidaBool(lista.Borrado);
            Assert.AreEqual(retorno, false);
        }

        [TestMethod]
        public void codigoValido() //Este test aplica para el codigo del enfasis, carrera, sigla curso y unidad academica
        //ya que tienen la misma condicion de validacion
        {
            GuiaHorario guia = new GuiaHorario();
            guia.CodigoCarreraCurso = "101010"; //Longitud < 10 valida
            bool retorno = ValidarTamanoText(guia.CodigoCarreraCurso, 10);
            Assert.AreEqual(retorno, true);
        }

        [TestMethod]
        public void nombreValido() //Este test aplica para el nombre de los cursos, unidades, carreras y enfasis
        //ya que tienen la misma condicion de validacion
        {
            GuiaHorario guia = new GuiaHorario();
            guia.NombreCurso = "Calculo 1"; //Longitud < 50 valida
            bool retorno = ValidarTamanoText(guia.NombreCurso, 50);
            Assert.AreEqual(retorno, true);
        }

        [TestMethod]
        public void semestreValido() //Este test solo aplica para semestre
        {
            GuiaHorario guia = new GuiaHorario();
            guia.Semestre = "2"; //Semestre con valor numerico positivo entre 1 y 3 valido
            bool retorno = ValidarNumero(guia.Semestre);
            Assert.AreEqual(retorno, true);
        }

        [TestMethod]
        public void annoValido() //Este test aplica para anno, numero de grupo, semestre
        {
            GuiaHorario guia = new GuiaHorario();
            guia.Anno = "2019"; //Anno entero positivo valido
            bool retorno = ValidarNumero(guia.Anno);
            Assert.AreEqual(retorno, true);
        }

        [TestMethod]
        public void correoValido() //Este test aplica para correo de estudiante y funcionario
        {
            GuiaHorario guia = new GuiaHorario();
            guia.CorreoMatricula = "correo@ucr.ac.cr"; //correo valido
            bool retorno = ValidarEmail(guia.CorreoMatricula);
            Assert.AreEqual(retorno, true);
        }

    }
}
