using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppIntegrador.Utilities
{
    /*Listado constantes de permisos.*/
    public enum Permission : int
    {
        /*Usuarios y perfiles*/
        VER_USUARIOS = 101,
        CREAR_USUARIOS,
        VER_DETALLES_USUARIOS,
        EDITAR_USUARIOS,
        BORRAR_USUARIOS,
        VER_PERMISOS_Y_PERFILES,
        EDITAR_PERMISOS_Y_PERFILES,
        ASIGNAR_PERMISOS_PERFILES,
        ASIGNAR_PERFILES_USUARIOS,

        /*Formularios*/
        CREAR_FORMULARIO = 201,
        VER_FORMULARIO,
        VER_DETALLES_FORMULARIO,
        EDITAR_FORMULARIO,
        BORRAR_FORMULARIO,
        CREAR_SECCION,
        VER_SECCION,
        EDITAR_SECCION,
        BORRAR_SECCION,
        CREAR_PREGUNTA,
        VER_PREGUNTAS,
        VER_DETALLES_PREGUNTA,
        EDITAR_PREGUNTA,
        BORRAR_PREGUNTA,
        LLENAR_FORMULARIO,

        /*Planes de mejora y objetivos*/
        VER_PLANES_MEJORA = 301,
        CREAR_PLANES_MEJORA,
        EDITAR_PLANES_MEJORA,
        BORRAR_PLANES_MEJORA,
        CREAR_OBJETIVOS,
        VER_OBJETIVOS,
        EDITAR_OBJETIVOS,
        BORRAR_OBJETIVOS,
        CREAR_ACCIONES_MEJORA,
        VER_ACCIONES_MEJORA,
        EDITAR_ACCIONES_MEJORA,
        BORRAR_ACCIONES_MEJORA,

        /*Visualización*/
        VER_RESPUESTAS_FORMULARIOS_PROPIOS = 401,
        VER_RESPUESTAS_FORMULARIOS_ENFASIS,

        /*Carga de datos*/
        CARGAR_DATOS_DESDE_CSV = 501

    };


    public interface IPerm
    {
        /// <summary>
        /// Verifica si el usuario en la sesión actual tiene el permiso indicado con la constante permission.
        /// </summary>
        /// <param name="permission">Número de permiso que se quiere verificar para el usuario actual.</param>
        /// <returns>True si el usuario tiene el permiso dentro de su perfil, carrera y énfasis, o false en caso contrario.</returns>
        bool IsAuthorized(Permission permission);

        /// <summary>Este método revisa si el usuario tiene el permiso asignado en el énfasis de la carrera.</summary>
        /// <param name="userMail">Nombre del usuario.</param>
        /// <param name="profileName">Nombre del perfil con que está trabajando.</param>
        /// <param name="majorCode">Código de la carrera.</param>
        /// <param name="emphasisCode">Código del énfasis de la carrera.</param>
        /// <param name="permissionId">Código identificador del permiso.</param>
        /// <returns>true si tiene el permiso, false en otro caso</returns>
        bool IsAllowed(string userMail, string profileName, string majorCode, string emphasisCode, Permission permissionId);

        /// <summary>Este método revisa si el usuario tiene el permiso asignado en el énfasis de la carrera.</summary>
        /// <param name="userMail">Nombre del usuario.</param>
        /// <param name="profileName">Nombre del perfil con que está trabajando.</param>
        /// <param name="majorCode">Código de la carrera.</param>
        /// <param name="permissionId">Código identificador del permiso.</param>
        /// <returns>true si tiene el permiso, false en otro caso</returns>
        bool IsAllowed(string userMail, string profileName, string majorCode, Permission permissionId);

        /// <summary>Este método revisa si el usuario tiene el permiso asignado en el énfasis de la carrera.</summary>
        /// <param name="userMail">Nombre del usuario.</param>
        /// <param name="profileName">Nombre del perfil con que está trabajando.</param>
        /// <param name="permissionId">Código identificador del permiso.</param>
        /// <returns>true si tiene el permiso, false en otro caso</returns>
        bool IsAllowed(string userMail, string profileName, Permission permissionId);

    }
}
