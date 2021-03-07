using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppIntegrador.Models
{
    public class ConfigViewHolder
    {
        private DataIntegradorEntities db = new DataIntegradorEntities();
        public List<Perfil> ListaPerfiles { get; set; }

        public List<Carrera> ListaCarreras { get; set; }

        public List<Enfasis> ListaEnfasis { get; set; }

        public List<SelectListItem> PerfilesSeleccionables { get; set; }

        public string NombreCarrera { get; set; }

        public string NombreEnfasis { get; set; }

        public ConfigViewHolder()
        {
            List<string> PerfilesString = new List<string>();
            ListaPerfiles = new List<Perfil>();
            ListaEnfasis = new List<Enfasis>();
            PerfilesSeleccionables = new List<SelectListItem>();
            using (var context = new DataIntegradorEntities())
            {
                var listaPerfiles = from Perfil in db.PerfilesXUsuario(CurrentUser.getUsername())
                                    select Perfil;
                foreach (var nombrePerfil in listaPerfiles) 
                    PerfilesString.Add(nombrePerfil.NombrePefil);


                foreach (string NombrePerfil in PerfilesString) {
                    ListaPerfiles.Add(db.Perfil.Find(NombrePerfil));
                    PerfilesSeleccionables.Add(new SelectListItem() { Text = NombrePerfil });
                }
            }
            Carrera carrera = db.Carrera.Find(CurrentUser.getUserMajorId());
            this.NombreCarrera = carrera.Nombre;
            Enfasis enfasis = db.Enfasis.Find(CurrentUser.getUserMajorId(), CurrentUser.getUserEmphasisId());
            this.NombreEnfasis = enfasis.Nombre;
        }
    }
}