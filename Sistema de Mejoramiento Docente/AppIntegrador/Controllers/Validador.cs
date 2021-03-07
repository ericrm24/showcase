using AppIntegrador.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AppIntegrador.Controllers
{
    public class Validador
    {
        protected bool ValidarTamanoText(String hilera, int tamano)
        {
            if (!String.IsNullOrEmpty(hilera))
            {
                if (hilera.Length <= tamano)
                {
                    return true;
                }
            }
            return false;
        }

        protected bool ValidarNumero(string valor)
        {
            if (!String.IsNullOrEmpty(valor))
            {
                if (int.TryParse(valor, out int intvalor) && intvalor > 0)
                {
                    return true;
                }
            }
            return false;
        }

        protected bool ValidaBool(string valor)
        {
            return false;
        }

        protected bool ValidarEmail(string email)
        {
            if (!String.IsNullOrEmpty(email)) {
            // source: http://thedailywtf.com/Articles/Validating_Email_Addresses.aspx
            Regex rx = new Regex(
            @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");
            if( rx.IsMatch(email))
            {
                return true;
            }
            System.Diagnostics.Debug.WriteLine("email1");
            }
            return false;
        }

        protected bool checkForSQLInjection(string userInput) // Basado en el codigo encontrado en http://aspdotnetmyblog.blogspot.com/2013/10/how-to-check-string-against-sql.html
        {
            bool isSQLInjection = false;
            string[] sqlCheckList = { //palabras reservadas SQL injection
                      "--",
                      ";--",
                      ";",
                      "/*",
                      "*/",
                      "@@",
                      "char ",
                      "nchar ",
                      "varchar ",
                      "nvarchar ",
                      "alter ",
                      "begin ",
                      "cast ",
                      "create ",
                      "cursor ",
                      "declare ",
                      "delete ",
                      "drop ",
                      "end ",
                      "exec ",
                      "execute ",
                      "fetch ",
                      "insert ",
                      "kill ",
                      "select ",
                      "sys ",
                      "sysobjects ",
                      "syscolumns ",
                      "table ",
                      "update "
                      };
                  string CheckString = userInput.Replace("'", "''");
                  for (int i = 0; i <= sqlCheckList.Length - 1; i++)
                  {
                      if ((CheckString.IndexOf(sqlCheckList[i],
                          StringComparison.OrdinalIgnoreCase) >= 0))
                      { isSQLInjection = true; }
                  }
            return isSQLInjection;
        }
    }

}
