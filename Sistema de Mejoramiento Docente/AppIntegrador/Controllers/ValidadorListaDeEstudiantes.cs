using AppIntegrador.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Controllers
{
    public class ValidadorListaDeEstudiantes : Validador
    {

        public Tuple<bool, string> Validar(ListaEstudiante lista, int filaActual)
        {
            int fila = filaActual;
            int columna = 0;
            string mensajeError = "";
            //Correo Persona
            columna++;
            if (!ValidarEmail(lista.CorreoPersona) || checkForSQLInjection(lista.CorreoPersona))
            {
                mensajeError = "El campo correo en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError); //email invalidao
            }

            //Id
            columna++;
            if (!ValidarTamanoText(lista.IdPersona, 30) || checkForSQLInjection(lista.IdPersona))
            {
                mensajeError = "El campo id persona en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            //Tipo Id
            columna++;
            if (!ValidarTamanoText(lista.TipoIdPersona, 30) || checkForSQLInjection(lista.TipoIdPersona))
            {
                mensajeError = "El campo tipo de id persona en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            //Nombre
            columna++;
            if (!ValidarTamanoText(lista.NombrePersona, 15) || checkForSQLInjection(lista.NombrePersona))
            {
                mensajeError = "El campo nombre persona en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            //Apellido
            columna++;
            if (!ValidarTamanoText(lista.ApellidoPersona, 15) || checkForSQLInjection(lista.ApellidoPersona))
            {
                mensajeError = "El campo apellido persona en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            //borrado
            columna++;
            if (checkForSQLInjection(lista.Borrado) || ValidaBool(lista.Borrado))
            {
                mensajeError = "El campo borrado en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            //Correo Estudiante
            columna++;
            if (!ValidarEmail(lista.CorreoEstudiante) || checkForSQLInjection(lista.CorreoEstudiante))
            {
                mensajeError = "El campo correo estudiante en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            //Correo estudiante empadronado
            columna++;
            if (!ValidarEmail(lista.CorreoEstudianteEmpadronado) || checkForSQLInjection(lista.CorreoEstudianteEmpadronado))
            {
                mensajeError = "El campo correo estudiante en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            //Codigo Carrera
            columna++;
            if (!ValidarTamanoText(lista.CodigoCarreraEmpadronado, 15) || checkForSQLInjection(lista.CodigoCarreraEmpadronado))
            {
                mensajeError = "El campo codigo de carrera en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            //Codigo Enfasis
            columna++;
            if (!ValidarTamanoText(lista.CodigoEnfasisEmpadronado, 15) || checkForSQLInjection(lista.CodigoEnfasisEmpadronado))
            {
                mensajeError = "El campo codigo de enfasis en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }
            return Tuple.Create(true, "");
        }

    }
}