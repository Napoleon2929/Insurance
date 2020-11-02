using Identity.Models;
using IdentityServer4.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shared.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using System.Security.Claims;

namespace Identity.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IPersistedGrantService _persistedGrantService;
        private readonly IIdentityServerInteractionService _interaction;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IPersistedGrantService persistedGrantService,
            IIdentityServerInteractionService interaction)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _persistedGrantService = persistedGrantService;
            _interaction = interaction;
        }


        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid login attempt.";
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["error"] = "Invalid login attempt.";
                return View(model);
            }

            var passwordCheckResult =
                   await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!passwordCheckResult.Succeeded)
            {
                TempData["error"] = "Invalid login attempt.";
                return View(model);
            }

            await _signInManager.SignInAsync(user, false);
            if(returnUrl == null)
            {
                return Redirect("/");
            }
            return Redirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            var subjectId = User?.Identity?.GetSubjectId();

            if ( model.LogoutId == null)
            {
                model.LogoutId = await _interaction.CreateLogoutContextAsync();
            }

            // delete authentication cookie
            await _signInManager.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)

            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);

            if (!string.IsNullOrEmpty(logout?.PostLogoutRedirectUri))
                return Redirect(logout.PostLogoutRedirectUri);

            return RedirectToAction(nameof(Login));
        }
    }
}
