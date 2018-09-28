using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppName.Web.Managers;
using AppName.Web.Models;
using AppName.Web.Repositories;
using AppName.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AppName.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHostingEnvironment _environment;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISignInManager _signInManager;

        public AccountController(
            IHostingEnvironment environment,
            IOptions<AppSettings> appSettings,
            IAuthenticationService authenticationService,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ISignInManager signInManager
            )
        {
            _environment = environment;
            _appSettings = appSettings;
            _authenticationService = authenticationService;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewData["ReturnUrl"] = returnUrl;
            
            var vm = new LoginViewModel
            {
                AvailableDomains = await GetDomains()
            };

            return View(vm);
        }

        private Task<List<SelectListItem>> GetDomains()
        {
            var domains = new List<SelectListItem>
            {
                new SelectListItem("yourdomain", "yourdomain"),
            };

            if (_environment.IsDevelopment())
            {
                domains.Insert(0, new SelectListItem("developer", "developer"));
            }

            return Task.FromResult(domains);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // validate the user credentials against the specified domain
                bool result = _authenticationService.ValidateUser(model.Domain, model.UserName, model.Password);
                if (result)
                {
                    // credentials are valid, now verify the user is someone the app expects to login
                    var user = await _userRepository.GetUserByUserNameAsync(model.Domain, model.UserName);
                    if (user != null && user.IsActive)
                    {
                        var roleNames = (await _roleRepository.GetRolesForUserAsync(user.Id)).Select(r => r.Name).ToList();
                        await _signInManager.SignInAsync(user, roleNames);

                        //user.LastLoginDate = _dateTime.Now;
                        //await _userRepository.UpdateUserAsync(user);
                        
                        if (!string.IsNullOrEmpty(returnUrl) && !string.Equals(returnUrl, "/") && Url.IsLocalUrl(returnUrl))
                        {
                            return RedirectToLocal(returnUrl);
                        }

                        //if (roleNames.Contains(Constants.RoleNames.Administrator))
                        //    return RedirectToAction(nameof(DashboardController.Index), "Dashboard", new { area = Constants.Areas.Administration });

                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError("", _appSettings.Value.Messages.AccessDenied);
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            model.AvailableDomains = await GetDomains();
            return View("Login", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Claims()
        {
            ViewBag.Title = "View claims";

            return View(HttpContext.User.Claims);
        }
    }
}