using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppIntegrador.Models
{
    public static class PermissionManagerViewBuilder
    {
        public static List<PersonProfileViewHolder> ListPersonProfiles(List<Persona> Persons)
        {
            List<PersonProfileViewHolder> list = new List<PersonProfileViewHolder>();
            foreach (Persona p in Persons) {
                list.Add(new PersonProfileViewHolder(p.Correo, p.HasProfileInEmph));
            }
            return list;
        }

        public static List<ProfilePermissionViewHolder> ListProfilePermissions(List<Permiso> Permissions)
        {
            List<ProfilePermissionViewHolder> list = new List<ProfilePermissionViewHolder>();
            foreach (Permiso p in Permissions) {
                list.Add(new ProfilePermissionViewHolder(p.Descripcion, p.Id, p.ActiveInProfileEmph));
            }
            return list;
        }
    }
}