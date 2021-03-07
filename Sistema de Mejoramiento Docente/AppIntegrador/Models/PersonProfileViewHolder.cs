using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models
{
    public class PersonProfileViewHolder
    {
        public string PersonMail { get; set; }

        public bool Checked { get; set; }

        public PersonProfileViewHolder(string personMail, bool Checked)
        {
            this.PersonMail = personMail;
            this.Checked = Checked;
        }
    }
}