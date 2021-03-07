using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AppIntegrador.Models;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using System.Threading.Tasks;
using AppIntegrador.Utilities;
using Security.Authentication;
using AppIntegrador.Helpers;

namespace AppIntegrador.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private DataIntegradorEntities db;
        private readonly IAuth auth;
        /*5 minutes timeout when an user fails to login 3 times in a row.*/
        private const int LOGIN_TIMEOUT = 300000;

        /*Max number of failed login attempts before temporarily locking the account.*/
        private const int MAX_FAILED_ATTEMPTS = 3;
        public HomeController()
        {
            db = new DataIntegradorEntities();
            auth = new FormsAuth();
        }

        public HomeController(DataIntegradorEntities db)
        {
            this.db = db;
            auth = new FormsAuth();
        }

        public HomeController(IAuth auth)
        {
            this.auth = auth;
            db = new DataIntegradorEntities();
        }

        public HomeController(DataIntegradorEntities db, IAuth auth)
        {
            this.db = db;
            this.auth = auth;
        }


        public ActionResult Index()
        {
            //Si hay informacion de alerta en TempData entonces pasarla al viewbag
            if (TempData["alertmessage"] != null)
            {
                ViewBag.typeMessage = "alert";
                ViewBag.AlertMessage = TempData["alertmessage"].ToString();
            }
            if (TempData["sweetalertmessage"] != null)
            {
                ViewBag.typeMessage = "sweetalertmessage";
                ViewBag.AlertMessage = TempData["sweetalertmessage"].ToString();
            }


            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");

            }
            return View("Index");
        }

        public ActionResult About()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            ViewBag.Message = "Your application description page.";
            return View("About");
        }

        public ActionResult Contact()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            ViewBag.Message = "Your contact page.";

            return View("Contact");
        }

        public ActionResult Login()
        {
            ViewBag.EnableBS4NoNavBar = true;
            //Verificamos si hay un mensaje de alerta de alguna de las operanciones realizadas, si lo hay lo desplegamos con javascript
            if (TempData["successMessage"] != null)
            {
                ViewBag.typeMessage = "success";
                ViewBag.NotifyMessage = TempData["successMessage"].ToString();
            }
            return View();
        }

        /* TAM 16.1 Servicio de captcha*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateGoogleCaptcha(type = "Login")] 
        public ActionResult Login(Usuario objUser, string returnUrl = null)
        {

            ViewBag.EnableBS4NoNavBar = true;
            if (ModelState.IsValid)
            {
                /*When loggin in, first checks whether the user's account is locked or deactivated.*/
                if (IsUserLocked(objUser))
                {
                    ModelState.AddModelError("Password", "Este usuario está bloqueado temporalmente.\nIntente de nuevo más tarde o contacte al adminstrador del sitio.");
                    return View(objUser);
                }

                /* Historia: Página de login -> Refactor del código para utilizar el modelo en el llamado al procedimiento almacenado */
                try
                {
                    ObjectParameter loginResult = new ObjectParameter("result", typeof(Int32));

                    // Se ejecuta el procedimiento almacenado
                    db.LoginUsuario(objUser.Username, objUser.Password, loginResult);
                    int result = (int)loginResult.Value;

                    // Credenciales correctos
                    if (result == 0)
                    {
                        /* Remueve el contador de intentos fallidos cuando el usuario logro entrar al sistema */
                        System.Web.HttpContext.Current.Application.Remove(objUser.Username);

                        /*Si la sesión puede ser configurada, es decir, el usuario tiene perfiles asociados, puede entrar.*/
                        if (ConfigureSession(objUser.Username))
                        {
                            auth.SetAuthCookie(objUser.Username, false);
                            if (returnUrl != null)
                            {
                                return Redirect(returnUrl);
                            }
                            return RedirectToAction("Index");
                        }
                        /*Sino, no puede entrar al sistema sin perfiles asociados.*/
                        else
                        {
                            ModelState.AddModelError("Password", "Esta cuenta no está debidamente configurada. Contacte al administrador del sitio.");
                        }
                    }
                    else
                    {
                        // No se encontró el usuario
                        if (result == 1)
                        {
                            ModelState.AddModelError("Password", "Usuario y/o contraseña incorrectos");
                        }
                        /*If user's account exists in the database but the password is wrong.*/
                        else
                        {
                            _ = WrongPassword(objUser);
                        }

                        return View("Login",objUser);
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex);
                }

            }
            return View(objUser);
        }

        /*User story TAM-1.3 Brute-force attack prevention.*/
        /* TAM 16.1 Servicio de captcha*/
        private ActionResult WrongPassword(Usuario objUser)
        {
            int failedAttempts = 0;
            ModelState.AddModelError("Password", "Usuario y/o contraseña incorrectos");

            /*If it's this user first failed login attempt, store it somewhere in the system to keep watching 
             this user's activity.*/
            if (System.Web.HttpContext.Current.Session["LoginFailures"] == null)
            {
                /*Sets the count of failed login attempts to 1.*/
                CurrentUser.setLoginFailures(1);
            }
            else
            {
                /*If this user has already made failed login attempts, increment the counter.*/            
                failedAttempts = (CurrentUser.getUserLoginFailures() + 1);

                if (failedAttempts > MAX_FAILED_ATTEMPTS)
                {
                    return View("Login", objUser);
                }
                            
                /*Save the failed attempts count back to somewhere in the system.*/                
                CurrentUser.setLoginFailures(failedAttempts);
            }      

            return null;
        }

        /*Function to tell whether a given user account is locked or not.*/
        private static bool IsUserLocked(Usuario objUser)
        {
            bool locked = false;
            using (var context = new DataIntegradorEntities())
            {
                var query = context.Usuario
                    .Where(u => u.Username == objUser.Username)
                    .FirstOrDefault<Usuario>();
                /*Just return the user account's active/inactive bit.*/
                if (query != null)
                    locked = !query.Activo;
            }
            return locked;
        }
        /*End of user story.*/

        [Authorize]
        public ActionResult Logout()
        {
            /*TAM 1.1.1: Modificado para que no guarde la sesión del usuario la siguiente vez que se ingrese al sistema.*/
            auth.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            CurrentUser.deleteCurrentUser(HttpContext.User.Identity.Name);
            return RedirectToAction("Index");
        }

        /* User story TAM-1.5 */
        public ActionResult PasswordReset()
        {
            ViewBag.EnableBS4NoNavBar = true;
            return View();
        }

        /* TAM 16.1 Servicio de captcha*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateGoogleCaptcha(type = "Always")]
        public ActionResult PasswordReset(string correo, bool enviarCorreo = true)
        {
            //var enlaceSeguro = db.EnlaceSeguro.Where(a => a.Hash == enlaceSeguroHash).FirstOrDefault();
            //var correo = enlaceSeguro.UsuarioAsociado;
            var user = db.Usuario.Where(a => a.Username == correo).FirstOrDefault();

            // Revisar también correo secundario
            if (user == null)
            {
                Persona correoAlt = db.Persona.Where(p => p.CorreoAlt == correo).FirstOrDefault();
                if (correoAlt != null)
                {
                    user = db.Usuario.Where(a => a.Username == correoAlt.Correo).FirstOrDefault();
                }
            }

            if (user != null)
            {
                /*Prevencion del bloqueo de Adminsitrador*/
                if (correo == "admin@mail.com")
                {
                    ViewBag.typeMessage = "alert";
                    ViewBag.NotifyMessage = "Este correo solo es para fines de desarrollo, no puede recibir un correo de recuperacion";
                    ViewBag.EnableBS4NoNavBar = true;
                    return View("PasswordReset");
                }
                EnlaceSeguroController enlaceController = new EnlaceSeguroController();

                //creamos un enlaceseguroanomimo de dos usos. Se ocupa que sea de dos ya que despues de utilizarlo para ser redireccionados a la pagina
                //de reestablecer contraseña se necesita que usos no este en 0 para poder hacer el cambio de contraseña propio.
                string enlaceParaReestablecer =  enlaceController.ObtenerEnlaceSeguroAnonimo(
                    "/Home/ReestablecerContrasenna/",usuario: user.Username, reestablecerContrasenna: true, usos: 2);

                //Creamos un timestamp para agregarlo al correo
                var timestamp = DateTime.Now;
                string fechaSalida = timestamp.ToString("dd/MM/yyyy"); //considerar añadir IFormatProvider
                string horaSalida = timestamp.ToString("hh:mm tt");

                //Enviamos un correo con el enlace y el timestamp
                EmailNotification notification = new EmailNotification();

                List<string> users = new List<string>();
                users.Add(correo);
                int notificationResult;
                if (enviarCorreo)
                {
                    notificationResult = notification.SendNotification(users, "Restablecimiento de contraseña",
                         "Se ha pedido un  restablecimiento de contraseña para el usuario: " + user.Username + ". El " + fechaSalida + " a las " + horaSalida + ". \n " +
                         "Si usted no realizó  la petición de restablecimiento puede ignorar este correo. Si no es la primera vez que recibe este correo por error, por favor \n" +
                         "contacte al administrador del sitio por medio de sistemamejoramientodocente@gmail.com. \n\n" +
                         "Siga este enlace para reestablecer su contraseña: " + enlaceParaReestablecer,
                         "Se ha pedido un  restablecimiento de contraseña para el usuario: " + user.Username + ". El " + fechaSalida + " a las " + horaSalida + ". " +
                         "Si usted no realizó  la petición de restablecimiento puede ignorar este correo. Si no es la primera vez que recibe este correo por error, por favor " +
                         "contacte al administrador del sitio por medio de sistemamejoramientodocente@gmail.com. <br><br>" +
                         "Siga este enlace para reestablecer su contraseña: " + enlaceParaReestablecer);
                }
                else 
                {
                    //solo para fines de testing
                    notificationResult = 0;
                }

                if (notificationResult == 0)
                {
                    TempData["successMessage"] = "Se ha enviado un correo para reestablecer su contraseña a la dirección indicada.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.typeMessage = "alert";
                    ViewBag.NotifyMessage = "No se pudo enviar el correo";
                    ViewBag.EnableBS4NoNavBar = true;
                    return View("PasswordReset");
                }
            }
            else
            {
                ViewBag.typeMessage = "alert";
                ViewBag.NotifyMessage = "Correo no encontrado";
            }
            ViewBag.EnableBS4NoNavBar = true;
            return View("PasswordReset");
        }

        public ActionResult ReestablecerContrasenna(string enlaceSeguroHash)
        {
            /*Si no entra por un enlaceSeguro, va para login.*/
            if(enlaceSeguroHash == null)
                return RedirectToAction("Login");

            ViewBag.EnableBS4NoNavBar = true;
            ViewBag.Hash = enlaceSeguroHash;
            return View("ReestablecerContrasenna");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReestablecerContrasenna(string receivedHash, string contrasennaNueva, string contrasennaConfirmar, bool enviarCorreo = true)
        {
            string receivedHashExtra = ViewBag.Hash;
            var enlaceSeguro = db.EnlaceSeguro.Where(a => a.Hash == receivedHash).FirstOrDefault();
            if (enlaceSeguro == null)
            {
                ViewBag.typeMessage = "alert";
                ViewBag.NotifyMessage = "El enlace de restablecimiento de contraseña no es valido, por favor obtenga otro enlace";
                return View();
            }

            DateTime momentoActual = DateTime.Now;
            DateTime expira = (DateTime)enlaceSeguro.Expira;
            int usos = enlaceSeguro.Usos;
            bool reestablecerContrasenna = enlaceSeguro.ReestablecerContrasenna;
            int fechaValida = DateTime.Compare(momentoActual, expira);

            if (usos != 0 && fechaValida < 0 && reestablecerContrasenna)
            {
                var correo = enlaceSeguro.UsuarioAsociado;
                var user = db.Usuario.Where(a => a.Username == correo).FirstOrDefault();
                if (contrasennaNueva != contrasennaConfirmar)
                {
                    ModelState.AddModelError("Password", "Las contraseña nueva y su confirmacion no son iguales.");
                }
                else
                {
                    db.ChangePassword(correo, contrasennaNueva);
                    //decrementamos los usos del enlace que acabamos de usar, estos se crean con dos usos en total, asi que debemos decrementarlo 
                    //aqui tambien.
                    db.EnlaceSeguro.Remove(enlaceSeguro);
                    db.SaveChanges();

                    //Enviamos un correo al usuario alertando del cambio
                    EmailNotification notification = new EmailNotification();

                    List<string> users = new List<string>();
                    users.Add(correo);

                    //Creamos un timestamp para agregarlo al correo
                    var timestamp = DateTime.Now;
                    string fechaSalida = timestamp.ToString("dd/MM/yyyy");
                    string horaSalida = timestamp.ToString("hh:mm tt");

                    if (enviarCorreo)
                    {
                        notification.SendNotification(users,
                            "Cambio de contraseña",
                            "Se ha realizado un cambio de contraseña para el usuario: " + user.Username + " . El " + fechaSalida + " a las " + horaSalida + ". \n " +
                            "Si usted no realizó este cambio por favor contactarse de inmediato con el administrador por medio de sistemamejoramientodocente@gmail.com",
                            "Se ha realizado un cambio de contraseña para el usuario: " + user.Username + " . El " + fechaSalida + " a las " + horaSalida + ". <br> " +
                            "Si usted no realizó este cambio por favor contactarse de inmediato con el administrador por medio de sistemamejoramientodocente@gmail.com");
                    }
                    ViewBag.typeMessage = "success";
                    ViewBag.NotifyTitle = "Contraseña Cambiada";
                    ViewBag.NotifyMessage = "Por seguridad se le va a redirigir al login.";
                }
            }
            else
            {
                ModelState.AddModelError("Username", "El enlace de restablecimiento de contraseña no es valido, por favor obtenga otro enlace");
            }
            ViewBag.EnableBS4NoNavBar = true;
            return View();
        }


        private bool ConfigureSession(string username)
        {
            ObjectParameter mejorPerfil = new ObjectParameter("PerfilPoderoso", typeof(string));
            ObjectParameter mejorCarrera = new ObjectParameter("CarreraPoderosa", typeof(string));
            ObjectParameter mejorEnfasis = new ObjectParameter("EnfasisPoderoso", typeof(string));
            db.SugerirConfiguracion(username, mejorPerfil, mejorCarrera, mejorEnfasis);

            /*Si el usuario no tiene perfiles asociados, no puede entrar al sistema. Se redirecciona a login con un mensaje de error.*/
            if (mejorPerfil.Value.Equals(DBNull.Value))
            {
                return false;
            }
            /*Configura la sesión del usuario con la selección que le da más valor: la combinación de perfil, carrera y énfasis
             donde tiene más permisos asignados. Sino tiene perfil asignado, se asigna Superusuario por defecto, para efectos de pruebas
             y no atrasar a los demás equipos.*/
            try
            {
                SetUserData(username, (string)mejorPerfil.Value, (string)mejorCarrera.Value, (string)mejorEnfasis.Value);
            } catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                return false;
            }
            return true;
        }

        /*TAM-3.1, 3.2 y 3.6: Función que guarda los datos relevantes del usuario loggeado para poder consultar
         la interfaz de permisos con esa información.*/
        private void SetUserData(string correoUsuario, string perfil, string codCarrera, string codEnfasis)
        {
            try
            {
                CurrentUser.setCurrentUser(correoUsuario, perfil, codCarrera, codEnfasis);
            }
            catch (Exception exception) 
            {
                Console.WriteLine(exception.ToString());
                throw;
            }
        }

        public ActionResult CambiarContrasenna()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            //ViewBag.Message = "Your application description page.";
            return View("CambiarContrasenna");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarContrasenna(string contrasennaActual, string contrasennaNueva, string contrasennaConfirmar)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            ObjectParameter loginResult = new ObjectParameter("result", typeof(Int32));

            // Se ejecuta el procedimiento almacenado
            db.LoginUsuario(CurrentUser.getUsername(), contrasennaActual, loginResult);
            if((int)loginResult.Value != 0)
            {
                ModelState.AddModelError("Username", "Contraseña Incorrecta.");
            }
            else 
            {
                if (contrasennaNueva != contrasennaConfirmar)
                {
                    ModelState.AddModelError("Password", "Las contraseña nueva y su confirmacion no son iguales.");
                }
                else
                {
                    db.ChangePassword(CurrentUser.getUsername(), contrasennaNueva);
                    db.SaveChanges();

                    //Enviamos un correo al usuario alertando del cambio
                    EmailNotification notification = new EmailNotification();

                    List<string> users = new List<string>();
                    users.Add(CurrentUser.getUsername());

                    //Creamos un timestamp para agregarlo al correo
                    var timestamp = DateTime.Now;
                    string fechaSalida = timestamp.ToString("dd/MM/yyyy");
                    string horaSalida = timestamp.ToString("hh:mm tt");

                    notification.SendNotification(users,
                        "Cambio de contraseña",
                        "Se ha realizado un cambio de contraseña para el usuario: " + CurrentUser.getUsername() + " . El " + fechaSalida + " a las " + horaSalida + ". \n " +
                        "Si usted no realizó este cambio por favor contactarse de inmediato con el administrador por medio de sistemamejoramientodocente@gmail.com",
                        "Se ha realizado un cambio de contraseña para el usuario: " + CurrentUser.getUsername() + " . El " + fechaSalida + " a las " + horaSalida + ". \n " +
                        "Si usted no realizó este cambio por favor contactarse de inmediato con el administrador por medio de sistemamejoramientodocente@gmail.com");

                    ViewBag.typeMessage = "success";
                    ViewBag.NotifyTitle = "Contraseña Cambiada";
                    ViewBag.NotifyMessage = "Por seguridad se le va a redirigir al login.";
                }
            }
            return View();
        }
    }
}