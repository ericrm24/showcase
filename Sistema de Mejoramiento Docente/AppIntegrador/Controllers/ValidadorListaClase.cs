using System;
using AppIntegrador.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Controllers
{
    public class ValidadorListaClase : Validador
    {

        public Tuple<bool, string> Validar(ListaClase lista, int filaActual)
        {
            int fila = filaActual;
            int columna = 0;
            string mensajeError = "";


            //Codigo Unidad
            columna++;
            if (!ValidarTamanoText(lista.CodigoUnidad, 10) || checkForSQLInjection(lista.CodigoUnidad))
            {
                mensajeError = "El campo codigo de unidad en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }
            //Nombre Facultad
            columna++;
            if (!ValidarTamanoText(lista.NombreFacultad, 50) || checkForSQLInjection(lista.NombreFacultad))
            {
                mensajeError = "El campo Nombre Facultad en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }
            //Codigo Unidad Carrera
            columna++;
            if (!ValidarTamanoText(lista.CodigoUnidadCarrera, 10) || checkForSQLInjection(lista.CodigoUnidadCarrera))
            {
                mensajeError = "El campo Codigo Unidad Carrera en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }
            //Codigo carrera Unidad
            columna++;
            if (!ValidarTamanoText(lista.CodigoCarreraUnidad, 10) || checkForSQLInjection(lista.CodigoCarreraUnidad))
            {
                mensajeError = "El campo Codigo carrera Unidad en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }
            //Codigo Carrera
            columna++;
            if (!ValidarTamanoText(lista.CodigoCarrera, 10) || checkForSQLInjection(lista.CodigoCarrera))
            {
                mensajeError = "El campo Codigo carrera en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }
            //Nombre Carrera
            columna++;
            if (!ValidarTamanoText(lista.NombreCarrera, 50) || checkForSQLInjection(lista.NombreCarrera))
            {
                mensajeError = "El campo nombre carrera en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }
            //Codigo Carrera Enfasis
            columna++;
            if (!ValidarTamanoText(lista.CodigoCarreraEnfasis, 10) || checkForSQLInjection(lista.CodigoCarreraEnfasis))
            {
                mensajeError = "El campo Codigo carrera enfasis en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }
            //Codigo Enfasis
            columna++;
            if (!ValidarTamanoText(lista.CodigoEnfasis, 10) || checkForSQLInjection(lista.CodigoEnfasis))
            {
                mensajeError = "El campo Codigo enfasis en la fila " + fila.ToString() + " , columna " + columna.ToString() + " es invalido";
                return Tuple.Create(false, mensajeError);
            }
            //Nombre Enfasis

            return Tuple.Create(true, "");
        }


    }
}