using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models
{
    public class UsuarioPersona
    {

        public Usuario Usuario { get; set; }

        public Persona Persona { get; set; }

    }

    public class TipoID
    {
        public TipoID(string text, string value) 
        {
            this.Text = text;
            this.Value = value;
        }
        public string Text { get; set; }
        public string Value { get; set; }
    }
}