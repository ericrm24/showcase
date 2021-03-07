using AppIntegrador.Helpers;
using AppIntegrador.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppIntegrador.Models
{
    public class PermissionsViewHolder
    {
        private DataIntegradorEntities db = new DataIntegradorEntities();

        private readonly IPerm permissionManager = new PermissionManager();
        public List<PerfilCodigo> Perfiles { get; set; }

        public List<Carrera> Carreras { get; set; }

        public List<Permiso> Permisos { get; set; }

        public List<Enfasis> EnfasisView { get; set; }

        public List<Persona> Personas{ get; set; }

        public MultiSelectList ListaCarreras { get; set; }

        public MultiSelectList ListaPermisos { get; set; }

        public MultiSelectList ListaEnfasis { get; set; }

        public MultiSelectList ListaPerfiles { get; set; }

        public MultiSelectList ListaPersonas { get; set; }

        public string CarrerasSeleccionadas { get; set; }

        public string EnfasisSeleccionados { get; set; }

        public bool PersonasSeleccionadas { get; set; }

        public bool PermisosSeleccionados { get; set; }

        public int PerfilesSeleccionados { get; set; }

        public PermissionsViewHolder()
        {
            List<Perfil> perfiles = db.Perfil.ToList();
            this.Perfiles = new List<PerfilCodigo>();
            int count = 0;
            foreach (Perfil perfil in perfiles)
            {
                this.Perfiles.Add(new PerfilCodigo(perfil.Nombre, count++));
            }

            List<Carrera> CarrerasUsuario = new List<Carrera>();
            List<Enfasis> EnfasisUsuario = new List<Enfasis>();

            using (var context = new DataIntegradorEntities())
            {
                /*TAM-11.1: En la página de administración de permisos y perfiles solo se muestran las carreras y énfasis en las que el usuario tiene postestad, en los dropdowns.*/
                var tuplas = from Dato in db.CarrerasYEnfasisXUsuarioXPerfil(CurrentUser.getUsername(), CurrentUser.getUserProfile())
                                    select Dato;
                foreach (var tupla in tuplas) {
                    Carrera carrera = new Carrera { Codigo = tupla.CodCarrera, Nombre = tupla.NombreCarrera };
                    if (!CarrerasUsuario.Contains(carrera) && (
                        permissionManager.IsAllowed(CurrentUser.getUsername(), CurrentUser.getUserProfile(), carrera.Codigo, Permission.ASIGNAR_PERFILES_USUARIOS) ||
                        permissionManager.IsAllowed(CurrentUser.getUsername(), CurrentUser.getUserProfile(), carrera.Codigo, Permission.ASIGNAR_PERMISOS_PERFILES)))
                        CarrerasUsuario.Add(carrera);

                    Enfasis enfasis = new Enfasis { CodCarrera = tupla.CodCarrera, Codigo = tupla.CodEnfasis, Nombre = tupla.NombreEnfasis };
                    if (!EnfasisUsuario.Contains(enfasis) && (
                        permissionManager.IsAllowed(CurrentUser.getUsername(), CurrentUser.getUserProfile(), carrera.Codigo, enfasis.Codigo, Permission.ASIGNAR_PERFILES_USUARIOS) ||
                        permissionManager.IsAllowed(CurrentUser.getUsername(), CurrentUser.getUserProfile(), carrera.Codigo, enfasis.Codigo, Permission.ASIGNAR_PERMISOS_PERFILES)))
                        EnfasisUsuario.Add(enfasis);
                }
            }

            this.Carreras = CarrerasUsuario.Distinct(new ComparadorCarreras()).ToList();
            this.EnfasisView = EnfasisUsuario.Distinct(new ComparadorEnfasis()).ToList();

            this.Permisos = db.Permiso.ToList();
            foreach (Permiso permiso in this.Permisos)
            {
                permiso.ActiveInProfileEmph = false;
            }
            /*Lista todas las personas en la base de datos del sistema, ordenadas por apellidos y luego por nombres.*/
            /*El usuario admin@mail.com no se muestra para que no se pueda quitar permisos él mismo. Por defecto ya 
             tiene todos los permisos disponibles.*/
            

            this.Personas = db.Persona.Where(item => !item.Borrado && item.Correo != "admin@mail.com").
                                       OrderBy(item => item.Apellido1).
                                       ThenBy(item => item.Apellido2).
                                       ThenBy(item => item.Nombre1).
                                       ThenBy(item => item.Nombre2).
                                       ToList();

            ObjectParameter resultado = new ObjectParameter("resultado", typeof(bool));
            List<Persona> personasEliminadas = new List<Persona>();
            foreach (Persona persona in this.Personas)
            {             
                persona.HasProfileInEmph = false;
            }

            this.ListaCarreras = GetCarreras();
            this.ListaPermisos = GetPermisos();
            this.ListaEnfasis = GetEnfasis();
            this.ListaPerfiles = GetPerfiles();
            this.ListaPersonas = GetPersonas();
            this.ConcatenarNombresPersonas();

        }

        public MultiSelectList GetCarreras()
        {
            return new MultiSelectList(this.Carreras, "Codigo", "Nombre", null);
        }
        public MultiSelectList GetPermisos()
        {
            return new MultiSelectList(this.Permisos, "Id", "Descripcion", null);
        }
        public MultiSelectList GetEnfasis()
        {
            return new MultiSelectList(this.EnfasisView, "Codigo", "Nombre", null);
        }
        public MultiSelectList GetPerfiles()
        {
            return new MultiSelectList(this.Perfiles, "Codigo", "NombrePerfil", null);
        }
        public MultiSelectList GetPersonas()
        {
            return new MultiSelectList(this.Personas, "Correo", "Nombre1", null);
        }

        private void ConcatenarNombresPersonas()
        {
            foreach (Persona p in this.Personas) {
                p.NombreCompleto = p.Apellido1 + " " + p.Apellido2 + " "+ p.Nombre1;
            }
        }
    }

    public class PerfilCodigo
    {
        public string NombrePerfil { get; set; }

        public int Codigo { get; set; }

        public PerfilCodigo(string perfil, int codigo)
        {
            this.NombrePerfil = perfil;
            this.Codigo = codigo;
        }
    }
}