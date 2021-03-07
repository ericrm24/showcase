using AppIntegrador.Utilities;
using Security.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppIntegrador.Models
{
    /*Clase para almacenar los datos de un usuario loggeado en el sistema. Guarda el nombre de usuario (correo),
     el perfil, el código de carrera y el código de énfasis asignado al iniciar sesión basado en la configuración
     que le genere más valor. Puede editarse si después el usuario desea cambiar su configuración.*/

    public static class CurrentUser
    {
        /*Numero máximo de fallos permitidos en login antes de bloquear la cuenta.*/
        private const int MAX_FAILED_ATTEMPTS = 3;

        public static string getUsername()
        {
            updateCurrentUser();
            return (string)HttpContext.Current.Session["Username"];
        }

        public static string getUserProfile()
        {
            updateCurrentUser();
            return (string)HttpContext.Current.Session["Profile"];
        }

        public static string getUserMajorId()
        {
            updateCurrentUser();
            return (string)HttpContext.Current.Session["MajorId"];
        }

        public static string getUserEmphasisId()
        {
            updateCurrentUser();
            return (string)HttpContext.Current.Session["EmphasisId"];
        }

        public static int getMaxUserLoginFailures()
        {
            return MAX_FAILED_ATTEMPTS;
        }

        public static int getUserLoginFailures()
        {
            if ((int?)HttpContext.Current.Session["LoginFailures"] != null)
                return (int)HttpContext.Current.Session["LoginFailures"];
            else
                return 0;
        }

        public static string getProfileImage()
        {
            return (string)HttpContext.Current.Session["ProfileImage"];
        }


        public static void setUserLoginFailures(int loginFailures)
        {
            HttpContext.Current.Session["LoginFailures"] = loginFailures;
        }


        public static void setUserProfile(string profile)
        {
            updateCurrentUser();
            HttpContext.Current.Session["Profile"] = profile;
            DataIntegradorEntities db = new DataIntegradorEntities();
            UsuarioActual user = new UsuarioActual();
            user = db.UsuarioActual.Find(getUsername());
            user.Perfil = profile;
            db.SaveChanges();
        }

        public static void setUserMajor(string major)
        {
            updateCurrentUser();
            HttpContext.Current.Session["MajorId"] = major;
            DataIntegradorEntities db = new DataIntegradorEntities();
            UsuarioActual user = new UsuarioActual();
            user = db.UsuarioActual.Find(getUsername());
            user.CodCarrera = major;
            db.SaveChanges();
        }

        public static void setUserEmphasis(string emphasis)
        {
            updateCurrentUser();
            HttpContext.Current.Session["EmphasisId"] = emphasis;
            DataIntegradorEntities db = new DataIntegradorEntities();
            UsuarioActual user = new UsuarioActual();
            user = db.UsuarioActual.Find(getUsername());
            user.CodEnfasis = emphasis;
            db.SaveChanges();
        }

        public static void setLoginFailures(int failures)
        {
            HttpContext.Current.Session["LoginFailures"] = failures;
        }

        /*Método que guarda en la base de datos los datos del usuario loggeado. Busca primero si ya está en la tabla,
        si ya está no hace nada y si no está lo inserta para configurar su sesión.*/
        public static void setCurrentUser(string username, string profile, string majorId, string emphasisId)
        {
            DataIntegradorEntities db = new DataIntegradorEntities();
            UsuarioActual newUser = new UsuarioActual();
            newUser.CorreoUsuario = username;
            newUser.Perfil = profile;
            newUser.CodCarrera = majorId;
            newUser.CodEnfasis = emphasisId;

            if (db.UsuarioActual.Find(username) == null)
            {
                try
                {
                    db.UsuarioActual.Add(newUser);
                    db.SaveChanges();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            }
            /* Codigo que no permite dos sesiones simultaneas*/

            // Preparar imagen de perfil predeterminada
            ProfilePicture picture = new ProfilePicture();
            Persona persona = db.Persona.Find(username);
            MemoryStream imagen = picture.GenerateCircle(persona.Nombre1, persona.Apellido1);
            string base64 = Convert.ToBase64String(imagen.ToArray());
            string imgSrc = string.Format("data:image/png;base64,{0}", base64);

            HttpContext.Current.Session["Username"] = username;
            HttpContext.Current.Session["Profile"] = profile;
            HttpContext.Current.Session["MajorId"] = majorId;
            HttpContext.Current.Session["EmphasisId"] = emphasisId;
            HttpContext.Current.Session["LoginFailures"] = 0;
            HttpContext.Current.Session["ProfileImage"] = imgSrc;
        }

        /*Limpia las variables globales de sesión al cerrar la sesión de un usuario.*/
        public static void clearSession()
        {
            try
            {
                HttpContext.Current.Session["Username"] = "";
                HttpContext.Current.Session["Profile"] = "";
                HttpContext.Current.Session["MajorId"] = "";
                HttpContext.Current.Session["EmphasisId"] = "";
                HttpContext.Current.Session["LoginFailures"] = 0;
                HttpContext.Current.Session["ProfileImage"] = "";
            } catch (NullReferenceException exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        /*Borra el usuario con la sesión iniciada de la tabla de UsuarioActual de la base de datos
         para cerrar su sesión.*/
        public static void deleteCurrentUser(string username)
        {
            DataIntegradorEntities db = new DataIntegradorEntities();
            UsuarioActual newUser = new UsuarioActual();
            newUser.CorreoUsuario = (username != null ? username : (string)HttpContext.Current.Session["Username"]);
            if (username == null)
            {
                Console.WriteLine("Error al borrar el usuario.");
                return;
            }
            else {
                UsuarioActual otroUsuario = db.UsuarioActual.Find(username);
                if (otroUsuario != null)
                {
                    newUser = otroUsuario;
                }
            }
            if (!db.UsuarioActual.Local.Contains(newUser))
            {
                db.UsuarioActual.Attach(newUser);
            }
            db.UsuarioActual.Remove(newUser);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            clearSession();
        }

        /*Borra todos los usuarios de la tabla UsuarioActual.*/
        public static void deleteAllUsers()
        {
            DataIntegradorEntities db = new DataIntegradorEntities();
            //Borra todos los usuarios en la tabla.
            db.UsuarioActual.RemoveRange(db.UsuarioActual.ToList());
            db.SaveChanges();
        }

        /*Método para actualizar desde base de datos los datos del usuario actual, en caso de que se borren
        automáticamente. */
        private static void updateCurrentUser()
        {
            string sessionUsername;
            try
            {
                sessionUsername = (string)HttpContext.Current.Session["Username"];
            }
            catch (NullReferenceException exception) 
            {
                Console.WriteLine(exception.ToString());
                throw new NullReferenceException("No existe la variable Session en el contexto actual.");
            }

            string contextUsername = HttpContext.Current.User.Identity.Name;

            if (sessionUsername == null || contextUsername != sessionUsername) {
                DataIntegradorEntities db = new DataIntegradorEntities();
                string name = System.Web.HttpContext.Current.User.Identity.Name;
                UsuarioActual user = db.UsuarioActual.Find(name);
                /*Si el usuario actual aún se encuentra en la base de datos, se vuelve a cargar en la sesión*/
                if (user != null)
                {
                    HttpContext.Current.Session["Username"] = user.CorreoUsuario;
                    HttpContext.Current.Session["Profile"] = user.Perfil;
                    HttpContext.Current.Session["MajorId"] = user.CodCarrera;
                    HttpContext.Current.Session["EmphasisId"] = user.CodEnfasis;
                }
                else /*Sino, se hace logout y se redirige a la pantalla de login.*/
                {
                    try
                    {
                        IAuth auth = new FormsAuth();
                        auth.SignOut();
                        clearSession();
                    } catch (NullReferenceException exception)
                    {
                        Console.WriteLine(exception.ToString());
                    }
                }
            }
        }
    }

}