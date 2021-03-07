/* Esta clase es necesaria para hacer mocking en los unit test de Forms Authentication */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Security.Authentication
{
    /// <summary>
    /// FormsAuthentication implementation of Authentication
    /// </summary>
    public class FormsAuth : IAuth
    {
        /// <summary>
        /// Gets the name of the cookie.
        /// </summary>
        public string CookieName
        {
            get { return FormsAuthentication.FormsCookieName; }
        }

        /// <summary>
        /// Gets the cookie path.
        /// </summary>
        public string CookiePath
        {
            get { return FormsAuthentication.FormsCookiePath; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return FormsAuthentication.IsEnabled; }
        }

        /// <summary>
        /// Gets the login URL.
        /// </summary>
        public string LoginUrl
        {
            get { return FormsAuthentication.LoginUrl; }
        }

        /// <summary>
        /// Gets a value indicating whether [require SSL].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require SSL]; otherwise, <c>false</c>.
        /// </value>
        public bool RequireSSL
        {
            get { return FormsAuthentication.RequireSSL; }
        }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return FormsAuthentication.Timeout; }
        }

        /// <summary>
        /// Authenticates the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public bool Authenticate(string name, string password)
        {
            return FormsAuthentication.Authenticate(name, password);
        }

        /// <summary>
        /// Decrypts the specified encrypted ticket.
        /// </summary>
        /// <param name="encryptedTicket">The encrypted ticket.</param>
        /// <returns></returns>
        public System.Web.Security.FormsAuthenticationTicket Decrypt(string encryptedTicket)
        {
            return FormsAuthentication.Decrypt(encryptedTicket);
        }

        /// <summary>
        /// Enables authentication.
        /// </summary>
        /// <param name="configurationData">The configuration data.</param>
        public void EnableAuthentication(System.Collections.Specialized.NameValueCollection configurationData)
        {
            FormsAuthentication.EnableFormsAuthentication(configurationData);
        }

        /// <summary>
        /// Encrypts the specified ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <returns></returns>
        public string Encrypt(System.Web.Security.FormsAuthenticationTicket ticket)
        {
            return FormsAuthentication.Encrypt(ticket);
        }

        /// <summary>
        /// Gets the auth cookie.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <returns></returns>
        public HttpCookie GetAuthCookie(string userName, bool createPersistentCookie)
        {
            return FormsAuthentication.GetAuthCookie(userName, createPersistentCookie);
        }

        /// <summary>
        /// Gets the auth cookie.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <param name="strCookiePath">The STR cookie path.</param>
        /// <returns></returns>
        public HttpCookie GetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath)
        {
            return FormsAuthentication.GetAuthCookie(userName, createPersistentCookie, strCookiePath);
        }

        /// <summary>
        /// Gets the redirect URL.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <returns></returns>
        public string GetRedirectUrl(string userName, bool createPersistentCookie)
        {
            return FormsAuthentication.GetRedirectUrl(userName, createPersistentCookie);
        }

        /// <summary>
        /// Hashes the password for storing in config file.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="passwordFormat">The password format.</param>
        /// <returns></returns>
        public string HashPasswordForStoringInConfigFile(string password, string passwordFormat)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(password, passwordFormat);
        }

        /// <summary>
        /// Redirects from login page.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        public void RedirectFromLoginPage(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.RedirectFromLoginPage(userName, createPersistentCookie);
        }

        /// <summary>
        /// Redirects from login page.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <param name="strCookiePath">The STR cookie path.</param>
        public void RedirectFromLoginPage(string userName, bool createPersistentCookie, string strCookiePath)
        {
            FormsAuthentication.RedirectFromLoginPage(userName, createPersistentCookie, strCookiePath);
        }

        /// <summary>
        /// Redirects to login page.
        /// </summary>
        /// <param name="extraQueryString">The extra query string.</param>
        public void RedirectToLoginPage(string extraQueryString)
        {
            FormsAuthentication.RedirectToLoginPage(extraQueryString);
        }

        /// <summary>
        /// Redirects to login page.
        /// </summary>
        public void RedirectToLoginPage()
        {
            FormsAuthentication.RedirectToLoginPage();
        }

        /// <summary>
        /// Sets authentication cookie.
        /// </summary>
        public void SetAuthCookie(string username, bool remember)
        {
            FormsAuthentication.SetAuthCookie(username, remember);
        }

        /// <summary>
        /// Signs out.
        /// </summary>
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}