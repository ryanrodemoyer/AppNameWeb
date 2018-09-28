using System;
using System.DirectoryServices.AccountManagement;

namespace AppName.Web.Services
{
    public interface IAuthenticationService
    {
        bool ValidateUser(string domain, string username, string password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// In Development mode, validate anything that comes through from the "developer" domain.
        /// </summary>
        /// <param name="domain">The domain to where the user should be validated against.</param>
        /// <param name="username">The domain user's username.</param>
        /// <param name="password">The domain user's password.</param>
        /// <returns></returns>
        public bool ValidateUser(string domain, string username, string password)
        {
            // TODO verify the domain is an acceptable value

            if (domain == "developer")
            {
                return true;
            }

            try
            {
                using (var adContext = new PrincipalContext(ContextType.Domain, domain))
                {
                    return adContext.ValidateCredentials(username, password);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
