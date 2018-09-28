using AppName.Web.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppName.Web.Managers
{
    public interface ISignInManager
    {
        Task SignInAsync(AppUser user, List<string> roles);

        Task SignOutAsync();
    }

    public class SignInManager : ISignInManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _environment;

        public SignInManager(
            IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment environment
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
        }

        public async Task SignInAsync(AppUser user, List<string> roleNames)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim("local/domain", user.Domain)
            };

            foreach (string roleName in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var identity = new ClaimsIdentity(claims, "local");

            // use the below line if you want to customize the name of the claim from
            // where the identity pulls the info
            //var identity = new ClaimsIdentity(claims, "local", "name", "role");

            var principal = new ClaimsPrincipal(identity);

            // setting IsPersistent to true creates a cookie where by default it expires in two weeks
            // this allows the user to stay logged in even after closing the browser
            // setting this to false means the cookie is deleted each time the browser is closed
            // create persistent logins when the environment is development but otherwise let's create temp sessions
            var props = new AuthenticationProperties { IsPersistent = _environment.IsDevelopment() };

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
        }

        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
