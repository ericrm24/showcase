using AppIntegrador.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace AppIntegrador.Utilities
{
    /*Interfaz de permisos para ser utilizada por las demás funcionalidades del sistema cuando se requiera
     saber si un usuario con un perfil en una carrera y un énfasis seleccionado tiene un determinado permiso.*/
    public class PermissionManager : IPerm
    {
        private DataIntegradorEntities db = new DataIntegradorEntities();

        /// <summary>Este método revisa si el usuario tiene el permiso asignado en el énfasis de la carrera.</summary>
        /// <param name="userMail">Nombre del usuario.</param>
        /// <param name="profileName">Nombre del perfil con que está trabajando.</param>
        /// <param name="majorCode">Código de la carrera.</param>
        /// <param name="emphasisCode">Código del énfasis de la carrera.</param>
        /// <param name="permissionId">Código identificador del permiso.</param>
        /// <returns>true si tiene el permiso, false en otro caso</returns>
        public bool IsAllowed(string userMail, string profileName, string majorCode, string emphasisCode, Permission permissionId)
        {
            ObjectParameter resultado = new ObjectParameter("resultado", typeof(bool));
            db.TienePermiso(userMail, profileName, majorCode, emphasisCode, (int) permissionId, resultado);
            return (bool) resultado.Value;
        }

        /// <summary>Este método revisa si el usuario tiene el permiso asignado en el énfasis de la carrera.</summary>
        /// <param name="userMail">Nombre del usuario.</param>
        /// <param name="profileName">Nombre del perfil con que está trabajando.</param>
        /// <param name="majorCode">Código de la carrera.</param>
        /// <param name="permissionId">Código identificador del permiso.</param>
        /// <returns>true si tiene el permiso, false en otro caso</returns>
        public bool IsAllowed(string userMail, string profileName, string majorCode, Permission permissionId)
        {
            ObjectParameter resultado = new ObjectParameter("resultado", typeof(bool));
            db.TienePermisoSinEnfasis(userMail, profileName, majorCode, (int) permissionId, resultado);
            return (bool)resultado.Value;
        }

        /// <summary>Este método revisa si el usuario tiene el permiso asignado en el énfasis de la carrera.</summary>
        /// <param name="userMail">Nombre del usuario.</param>
        /// <param name="profileName">Nombre del perfil con que está trabajando.</param>
        /// <param name="permissionId">Código identificador del permiso.</param>
        /// <returns>true si tiene el permiso, false en otro caso</returns>
        public bool IsAllowed(string userMail, string profileName, Permission permissionId)
        {
            ObjectParameter resultado = new ObjectParameter("resultado", typeof(bool));
            db.TienePermisoSinEnfasisNiCarrera(userMail, profileName, (int) permissionId, resultado);
            return (bool)resultado.Value;
        }

        private bool IsUserAuthorized(Permission permissionId)
        {
            /*Both major and emphasis are null*/
            if(CurrentUser.getUserEmphasisId() == null && CurrentUser.getUserMajorId() == null)
                return IsAllowed(
                CurrentUser.getUsername(),
                CurrentUser.getUserProfile(),
                permissionId
            );
            /*Only emphasis is null*/
            else if (CurrentUser.getUserEmphasisId() == null)
                return IsAllowed(
                CurrentUser.getUsername(),
                CurrentUser.getUserProfile(),
                CurrentUser.getUserMajorId(),
                permissionId
            );
            /*All parameters supplied*/
            else
            return this.IsAllowed(
                CurrentUser.getUsername(),
                CurrentUser.getUserProfile(),
                CurrentUser.getUserMajorId(),
                CurrentUser.getUserEmphasisId(),
                permissionId
            );
        }

        /// <summary>
        /// Verifica si el usuario en la sesión actual tiene el permiso indicado con la constante permission.
        /// </summary>
        /// <param name="permission">Número de permiso que se quiere verificar para el usuario actual.</param>
        /// <returns>True si el usuario tiene el permiso dentro de su perfil, carrera y énfasis, o false en caso contrario.</returns>
        public bool IsAuthorized(Permission permission)
        {
            /*Superusuario tiene permiso para todo, es innecesario consultarlo en la base de datos.*/
            if (CurrentUser.getUserProfile() == "Superusuario")
                return true;
            else
                return new PermissionManager().IsUserAuthorized(permission);
        }
    }

}