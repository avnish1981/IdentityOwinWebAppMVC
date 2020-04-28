using IdentityOwinWebApp.Demo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IdentityOwinWebApp.Demo.Controllers
{
    public class AccountController : Controller
    {
        public UserManager<IdentityUser> UserManager => HttpContext.GetOwinContext().Get<UserManager<IdentityUser>>();
        public SignInManager<IdentityUser, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<IdentityUser,string>>();
        // GET: Account

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost ]
        public async Task<ActionResult> Login(Login login )
        {
           var signInStatus =  await SignInManager.PasswordSignInAsync(login.UserName, login.Password, true, true);
            switch(signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                default:
                    ModelState.AddModelError("", "Invalid Crendentials");
                    return View(login);
            }
            
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(Register register)
        {
            var idenetityUser = await UserManager.FindByNameAsync(register.UserName);
            if(idenetityUser!=null)
            {
                return RedirectToAction("Index", "Home");
            }
          var identityResult =  await  UserManager.CreateAsync(new IdentityUser(register.UserName ),register.Password );
            if(identityResult.Succeeded )
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", identityResult.Errors.FirstOrDefault());
            return View(register);
            
        }
    }
}