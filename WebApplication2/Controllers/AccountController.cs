using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApplication2.Infrastructure;
using WebApplication2.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                AuthManager.SignOut();
                return View("Error", new string[] { "В доступе отказано" });
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { UserName = model.Name, Email = model.Email, lastLogin = DateTime.Now, registerDate = DateTime.Now };
                IdentityResult result =
                    await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    IdentityResult result2 = UserManager.AddToRole(user.Id, "Unblock");
                    if (result2.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                }
                AddErrorsFromResult(result);
            }
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel details)
        {
            if (details.Name == null || details.Password == null)
            {
                return View(details);
            }
            else
            {

                AppUser user = await UserManager.FindAsync(details.Name, details.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Некорректное имя или пароль.");
                }
                else
                {
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                        DefaultAuthenticationTypes.ApplicationCookie);

                    user.lastLogin = DateTime.Now;
                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, ident);
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Home");
                }

                return View(details);
            }
        }

        public ActionResult Logout()
        {
            AuthManager.SignOut();
            return RedirectToAction("Login", "Account");
        }
        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}