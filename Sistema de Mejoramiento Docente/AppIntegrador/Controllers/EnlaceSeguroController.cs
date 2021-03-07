using AppIntegrador.Models;
using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;

namespace AppIntegrador.Controllers
{
    public class EnlaceSeguroController : Controller
    {
        private DataIntegradorEntities db;
        
        // GET: EnlaceSeguro
        public EnlaceSeguroController()
        {
            db = new DataIntegradorEntities();
        }

        public EnlaceSeguroController(DataIntegradorEntities db)
        {
            this.db = db;
        }

        [AllowAnonymous]
        public ActionResult RedireccionSeguraAnonymous(string urlHash)
        {
            return Redirigir(urlHash, true);            
        }

        public ActionResult RedireccionSegura(string urlHash)
        {
            return Redirigir(urlHash);
        }

        [AllowAnonymous]
        private ActionResult Redirigir(string urlHash, bool anonimo = false)
        {
            // Busca tupla en la base de datos
            EnlaceSeguro enlaceSeguro = db.EnlaceSeguro.SingleOrDefault(u => u.Hash == urlHash);
            if (enlaceSeguro != null)
            {
                // Podría mejorarse para asegurarse de que la hora esté sincronizada con la base de datos
                DateTime momentoActual = DateTime.Now;
                DateTime expira = (DateTime)enlaceSeguro.Expira;
                int usos = enlaceSeguro.Usos;
                bool reestablecerContrasenna = enlaceSeguro.ReestablecerContrasenna;
                int fechaValida = DateTime.Compare(momentoActual, expira);
                // Enlace válido
                if (fechaValida < 0)
                {
                    // no se utiliza "> 0" ya que se desea que -1 haga que el enlace no tenga limite de uso
                    //un enlace seguro necesita de dos usos como minimo para poder establecer una nueva contraseña
                    //ya que se decrementa al redirijir y al cambiar la contraseña para un total de dos usos.
                    if (usos != 0 && !(usos == 1 && reestablecerContrasenna)) {
                        // Revisar usuario válido
                        if (!anonimo && enlaceSeguro.UsuarioAsociado != null)
                        {
                            if (CurrentUser.getUsername() == enlaceSeguro.UsuarioAsociado)
                            {
                                //se envia a borrar el enlace, esto decrementa su valor de "usos" disponibles.
                                db.EnlaceSeguro.Remove(enlaceSeguro);
                                db.SaveChanges();
                                return Redirect(enlaceSeguro.UrlReal);
                            }
                        }
                        // Enlace no relacionado a un solo usuario o anónimos permitidos
                        else
                        {
                            if (!reestablecerContrasenna)
                            {
                                //se envia a borrar el enlace, esto decrementa su valor de "usos" disponibles.
                                db.EnlaceSeguro.Remove(enlaceSeguro);
                                db.SaveChanges();
                                return Redirect(enlaceSeguro.UrlReal);
                            }
                            else
                            //Los enlaces seguros para reestablecer una contraseña se comportan de una manera diferente
                            //y se manejan en el controlador de Home, en "ReestablecerContrasenna".
                            {
                                //se envia a borrar el enlace, esto decrementa su valor de "usos" disponibles.
                                db.EnlaceSeguro.Remove(enlaceSeguro);
                                db.SaveChanges();
                                return RedirectToAction("ReestablecerContrasenna", "Home", new { enlaceSeguroHash = enlaceSeguro.Hash });
                            }
                        }
                    } else if (reestablecerContrasenna)
                    {
                        TempData["alertmessage"] = "Enlace no válido.";
                        return RedirectToAction("Login", "Home");
                    }
                }   
            }
            TempData["alertmessage"] = "Enlace no válido.";
            return RedirectToAction("Index", "Home");
        }

        public string ObtenerEnlaceSeguro(string urlReal, string usuario = null, DateTime? expira = null, int usos = 0)
        {
            return GenerarEnlaceSeguro(urlReal, usuario, expira, usos);
        }

        public string ObtenerEnlaceSeguroAnonimo(string urlReal, DateTime? expira = null, int usos = 0, bool reestablecerContrasenna = false, string usuario = null)
        {
            return GenerarEnlaceSeguro(urlReal, usuario, expira, usos, true, reestablecerContrasenna);
        }

        private string GenerarEnlaceSeguro(string urlReal, string usuario = null, DateTime? expira = null, int usos = 0, bool anonimo = false, bool reestablecerContrasenna = false)
        {
            // Almacenar los datos necesitados
            ObjectParameter resultadoHash = new ObjectParameter("resultadohash", typeof(string));
            ObjectParameter estado = new ObjectParameter("estado", typeof(string));
            db.AgregarEnlaceSeguro(usuario, urlReal, expira, usos, reestablecerContrasenna, resultadoHash, estado);

            // Obtener los datos que debe recibir quien solicitó el enlace
            string urlHash = (string) resultadoHash.Value;
            var request = System.Web.HttpContext.Current.Request;
            string domain = request.Url.Scheme + Uri.SchemeDelimiter + request.Url.Authority;

            if (anonimo)
            {
                return domain + "/EnlaceSeguro/RedireccionSeguraAnonymous?urlHash=" + urlHash;
            }
            else
            {
                return domain + "/EnlaceSeguro/RedireccionSegura?urlHash=" + urlHash;
            }
        }
    }
}