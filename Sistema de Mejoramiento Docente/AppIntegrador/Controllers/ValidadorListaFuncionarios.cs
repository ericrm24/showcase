using AppIntegrador.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Controllers
{
    public class ValidadorListaFuncionarios : Validador
    {
        public Tuple<bool, string> Validar(ListaFuncionario lista, int filaActual)
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

            //Correo funcionario
            columna++;
            if (!ValidarEmail(lista.CorreoFuncionario) || checkForSQLInjection(lista.CorreoFuncionario))
            {
                mensajeError = "El campo correo funcionario en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            //Correo Profe
            columna++;
            if (!ValidarEmail(lista.CorreoProfesor) || checkForSQLInjection(lista.CorreoProfesor))
            {
                mensajeError = "El campo correo profesor en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            //Correo funcionario Trabaja
            columna++;
            if (!ValidarEmail(lista.CorreoFuncionarioTrabaja) || checkForSQLInjection(lista.CorreoFuncionarioTrabaja))
            {
                mensajeError = "El campo correo funcionario en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            //CODIGO UNIDAD ACADEMICA
            columna++;
            if (!ValidarTamanoText(lista.CodigoUnidadTrabaja, 15) || checkForSQLInjection(lista.CodigoUnidadTrabaja))
            {
                mensajeError = "El campo código unidad persona en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }

            return Tuple.Create(true, "");
        }
    }
}