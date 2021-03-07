using AppIntegrador.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Helpers
{
    public class ComparadorCarreras : IEqualityComparer<Carrera>
    {

        public bool Equals(Carrera x, Carrera y)
        {
            if (x != null && y != null)
                return x.Codigo == y.Codigo;
            else
                return false;
        }

        public int GetHashCode(Carrera obj)
        {
            int result = 0;
            try
            {
                result = int.Parse(obj.Codigo);
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
                result = -1;
            }
            return result;
        }
        

    }
    public class ComparadorEnfasis : IEqualityComparer<Enfasis>
    {

        public bool Equals(Enfasis x, Enfasis y)
        {
            if (x != null && y != null)
                return x.Codigo == y.Codigo;
            else
                return false;
        }

        public int GetHashCode(Enfasis obj)
        {
            int result = 0;
            try
            {
                result = int.Parse(obj.Codigo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                result = -1;
            }
            return result;
        }

    }
}