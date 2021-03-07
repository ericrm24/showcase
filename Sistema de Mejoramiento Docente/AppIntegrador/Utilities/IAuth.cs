/* Esta clase es necesaria para hacer mocking en los unit test de Forms Authentication */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Security.Authentication
{
    /// <summary>
    /// An interface used for authentication
    /// </summary>
    public interface IAuth
    {
        /// <summary>
        /// Gets the name of the cookie.
        /// </summary>
        string CookieName { get; }

        /// <summary>
        /// Gets the cookie path.
        /// </summary>
        string CookiePath { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsEnabled { get; }

        /// <summary>
        /// Gets the login URL.
        /// </summary>
        string LoginUrl { get; }

        /// <summary>
        /// Gets a value indicating whether [require SSL].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [require SSL]; otherwise, <c>false</c>.
        /// </value>
        bool RequireSSL { get; }

        /// <summary>
        /// Gets the timeout.
        /// </summary>
        TimeSpan Timeout { get; }


        /// <summary>
        /// Validates a user name and password against credentials stored in the configuration
        //  file for an application.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="password">The password.</param>
        bool Authenticate(string name, string password);

        /// <summary>
        /// Decrypts the specified encrypted ticket.
        /// </summary>
        /// <param name="encryptedTicket">The encrypted ticket.</param>
        /// <returns></returns>
        FormsAuthenticationTicket Decrypt(string encryptedTicket);

        /// <summary>
        /// Enables authentication.
        /// </summary>
        /// <param name="configurationData">The configuration data.</param>
        void EnableAuthentication(NameValueCollection configurationData);

        /// <summary>
        /// Encrypts the specified ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        /// <returns></returns>
        string Encrypt(FormsAuthenticationTicket ticket);

        /// <summary>
        /// Gets the auth cookie.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <returns></returns>
        HttpCookie GetAuthCookie(string userName, bool createPersistentCookie);

        /// <summary>
        /// Gets the auth cookie.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <param name="strCookiePath">The STR cookie path.</param>
        /// <returns></returns>
        HttpCookie GetAuthCookie(string userName, bool createPersistentCookie, string strCookiePath);

        /// <summary>
        /// Gets the redirect URL.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <returns></returns>
        string GetRedirectUrl(string userName, bool createPersistentCookie);

        /// <summary>
        /// Hashes the password for storing in config file.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="passwordFormat">The password format.</param>
        /// <returns></returns>
        string HashPasswordForStoringInConfigFile(string password, string passwordFormat);

        /// <summary>
        /// Redirects from login page.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        void RedirectFromLoginPage(string userName, bool createPersistentCookie);

        /// <summary>
        /// Redirects from login page.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <param name="strCookiePath">The STR cookie path.</param>
        void RedirectFromLoginPage(string userName, bool createPersistentCookie, string strCookiePath);

        /// <summary>
        /// Redirects to login page.
        /// </summary>
        /// <param name="extraQueryString">The extra query string.</param>
        void RedirectToLoginPage(string extraQueryString);

        /// <summary>
        /// Redirects to login page.
        /// </summary>
        void RedirectToLoginPage();

        /// <summary>
        /// Sets authentication cookie.
        /// </summary>
        void SetAuthCookie(string username, bool remember);

        /// <summary>
        /// Signs out.
        /// </summary>
        void SignOut();
    }
}