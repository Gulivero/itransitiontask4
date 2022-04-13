using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Infrastructure;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Unblock")]
    public class HomeController : Controller
    {
        AppIdentityDbContext db = new AppIdentityDbContext();
        public ActionResult Index()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var user = UserManager.FindById(userId);
            if (user == null)
            {
                return View("Error", new string[] { "Ваш аккаунт был удалён" });
            }
            if (UserManager.IsInRole(userId, "Block"))
            {
                return View("Error", new string[] { "Ваш аккаунт был заблокирован" });
            }
            user.lastLogin = DateTime.Now;
            IdentityResult result = UserManager.Update(user);
            if (result.Succeeded)
            {
                return View(new UsersAndRolesModel { Users = UserManager.Users, Roles = RoleManager.Roles, Messages = db.Messages });
            }
            else
            {
                return View("Error", result.Errors);
            }

        }

        [HttpPost]
        public async Task<ActionResult> Index(FormCollection formCollection, string action)
        {
            IdentityResult result;
            if (formCollection.GetValue("foo") != null)
            {
                string[] ids = formCollection["foo"].Split(new char[] { ',' });
                foreach (string id in ids)
                {
                    if (action == "Delete")
                    {
                        var user = await UserManager.FindByIdAsync(id);
                        if (user != null)
                        {
                            result = await UserManager.DeleteAsync(user);
                            if (!result.Succeeded)
                            {
                                return View("Error", result.Errors);
                            }
                        }
                    }
                    else if (action == "Unblock")
                    {
                        if (UserManager.IsInRole(id, "Block"))
                        {
                            result = await UserManager.RemoveFromRoleAsync(id, "Block");
                            if (!result.Succeeded)
                            {
                                return View("Error", result.Errors);
                            }

                            result = await UserManager.AddToRoleAsync(id, "Unblock");

                            if (!result.Succeeded)
                            {
                                return View("Error", result.Errors);
                            }
                        }
                    }
                    else if (action == "Block")
                    {
                        if (UserManager.IsInRole(id, "Unblock"))
                        {
                            result = await UserManager.RemoveFromRoleAsync(id, "Unblock");
                            if (!result.Succeeded)
                            {
                                return View("Error", result.Errors);
                            }

                            result = await UserManager.AddToRoleAsync(id, "Block");

                            if (!result.Succeeded)
                            {
                                return View("Error", result.Errors);
                            }
                        }
                    }

                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Send(string action, string username, string subject, string messageText)
        {
            if (action == "send")
            {
                var user = await UserManager.FindByNameAsync(username);
                if (user != null)
                {
                    Message message = new Message() 
                    { 
                        Subject = subject, 
                        Text = messageText, 
                        SenderId = HttpContext.User.Identity.GetUserId(), 
                        Created = DateTime.Now, 
                        RecieverId = user.Id
                    };

                    db.Messages.Add(message);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        private AppRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
            }
        }
    }
}