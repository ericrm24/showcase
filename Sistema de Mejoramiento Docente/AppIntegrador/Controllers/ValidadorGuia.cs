using AppIntegrador.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Controllers
{
    public class ValidadorGuia : Validador
    {

        public Tuple<bool, string> Validar(GuiaHorario guia, int filaActual)
        {
            int fila = filaActual;
            int columna = 0;
            string mensajeError = "";
            //Codigo Carrera
            columna++;
            if (!ValidarTamanoText(guia.CodigoCarreraCurso, 10) || checkForSQLInjection(guia.CodigoCarreraCurso))
            {
                mensajeError = "El campo codigo de carrera en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Codigo Enfasis
            columna++;
            if (!ValidarTamanoText(guia.CodigoEnfasisCurso, 10) || checkForSQLInjection(guia.CodigoEnfasisCurso))
            {
                mensajeError = "El campo codigo de enfasis en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Sigla Curso Carrera
            columna++;
            if (!ValidarTamanoText(guia.SiglaCursoCarrera, 10) || checkForSQLInjection(guia.SiglaCursoCarrera))
            {
                mensajeError = "El campo sigla curso en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Sigla Curso
            columna++;
            if (!ValidarTamanoText(guia.SiglaCursoCarrera, 10) || checkForSQLInjection(guia.SiglaCursoCarrera))
            {
                mensajeError = "El campo sigla curso en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Nombre Curso
            columna++;
            if (!ValidarTamanoText(guia.NombreCurso, 50) || checkForSQLInjection(guia.NombreCurso))
            {
                mensajeError = "El campo nombre curso en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Sigla Curso Grupo
            columna++;
            if (!ValidarTamanoText(guia.SiglaCursoGrupo, 10) || checkForSQLInjection(guia.SiglaCursoGrupo))
            {
                mensajeError = "El campo sigla curso en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Num Grupo
            columna++;
            if (!ValidarNumero(guia.NumeroGrupo) || checkForSQLInjection(guia.NumeroGrupo))
            {
                mensajeError = "El campo numero de grupo en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Semestre
            columna++;
            if (!ValidarNumero(guia.Semestre) || checkForSQLInjection(guia.Semestre))
            {
                mensajeError = "El campo semestre en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Anno
            columna++;
            if (!ValidarNumero(guia.Anno) || checkForSQLInjection(guia.Anno))
            {
                mensajeError = "El campo anio en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }


            //Sigla Curso Matricula
            columna++;
            if (!ValidarTamanoText(guia.SiglaCursoMatricula, 10) || checkForSQLInjection(guia.SiglaCursoMatricula))
            {
                mensajeError = "El campo sigla curso en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Num Grupo Matricula
            columna++;
            if (!ValidarNumero(guia.NumeroGrupoMatricula) || checkForSQLInjection(guia.NumeroGrupoMatricula))
            {
                mensajeError = "El campo numero de grupo en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Semestre Imparte
            columna++;
            if (!ValidarNumero(guia.SemestreMatricula) || checkForSQLInjection(guia.SemestreMatricula))
            {
                mensajeError = "El campo semestre en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Anno Imparte
            columna++;
            if (!ValidarNumero(guia.AnnoMatricula) || checkForSQLInjection(guia.AnnoMatricula))
            {
                mensajeError = "El campo anio en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Correo Matricula
            columna++;
            if (!ValidarEmail(guia.CorreoMatricula) || checkForSQLInjection(guia.CorreoMatricula))
            {
                mensajeError = "El campo correo estudiante en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }            
            return Tuple.Create(true, "");
        }
    }
}