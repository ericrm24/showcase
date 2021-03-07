using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models
{
    public class ProfilePermissionViewHolder
    {
        public string PermissionName { get; set; }

        public int PermissionCode { get; set; }

        public bool Checked { get; set; }

        public ProfilePermissionViewHolder(string PermissionName, int PermissionCode, bool Checked)
        {
            this.PermissionName = PermissionName;
            this.PermissionCode = PermissionCode;
            this.Checked = Checked;
        }
    }
}