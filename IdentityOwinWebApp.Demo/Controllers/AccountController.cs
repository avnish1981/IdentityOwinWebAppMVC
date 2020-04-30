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
using Microsoft.Owin.Security;
using static System.Web.Razor.Parser.SyntaxConstants;


namespace IdentityOwinWebApp.Demo.Controllers
{
    public class AccountController : Controller
    {
        //public UserManager<IdentityUser> UserManager => HttpContext.GetOwinContext().Get<UserManager<IdentityUser>>();
        //public SignInManager<IdentityUser, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<IdentityUser, string>>();
        //Extending User Configuration
        public UserManager<ExtendingUser> UserManager => HttpContext.GetOwinContext().Get<UserManager<ExtendingUser>>();
        public SignInManager<ExtendingUser, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<ExtendingUser, string>>();
        // GET: Account
       
       public async  Task<ActionResult> ExternalCallback(string providerName)
        {
            var loginInfo = await SignInManager.AuthenticationManager.GetExternalLoginInfoAsync();
            var signInStatus = await SignInManager.ExternalSignInAsync(loginInfo, true);
            switch (signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                default:
                    var existingUser = await UserManager.FindByNameAsync(loginInfo.Email);
                    if (existingUser != null)
                    {
                        var result = await UserManager.AddLoginAsync(existingUser.Id, loginInfo.Login);
                        if (result.Succeeded)
                        {
                            return await ExternalCallback(providerName);
                        }
                    }
                    return View("Error");
            }

           

        }

        public ActionResult ExternalAuthentication(string provider)
        {
            SignInManager.AuthenticationManager.Challenge(new AuthenticationProperties

            {
                RedirectUri = Url.Action("ExternalCallback", new { provider})

            },provider );
            return new HttpUnauthorizedResult();
        }

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
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("ChooseProvider");
                default:
                    ModelState.AddModelError("", "Invalid Crendentials");
                    return View(login);
            }
            
        }
        public async  Task<ActionResult > ChooseProvider()
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            var providers = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            return View(new ChooseProvider { Providers = providers.ToList() });
        }
        [HttpPost ]
        public async Task<ActionResult > ChooseProvider(ChooseProvider choose )
        {
            await SignInManager.SendTwoFactorCodeAsync(choose.ChosenProvider);
            return RedirectToAction("Twofactor", new { provider = choose.ChosenProvider });
        }
        public ActionResult TwoFactor(string providerName)
        {
            return View(new TwoFactor {Provider=providerName  });
        }
        [HttpPost ]
        public async Task<ActionResult> TwoFactor(TwoFactor two )
        {
            var signInStatus = await SignInManager.TwoFactorSignInAsync (two.Provider, two.Code, true, false);
            switch(signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                default:
                    ModelState.AddModelError("", "Invalid Crendentials");
                    return View(two);
            }

        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(Register register)
        {
            var idenetityUser = await UserManager.FindByNameAsync(register.Email);
            if (idenetityUser != null)
            {
                return RedirectToAction("Index", "Home");
            }
            //var identityResult = await UserManager.CreateAsync(new IdentityUser(register.UserName), register.Password);
            //if (identityResult.Succeeded)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //ModelState.AddModelError("", identityResult.Errors.FirstOrDefault());
            //return View(register);
            else
            {
                var user = new ExtendingUser
                {

                    UserName  = register.UserName ,
                    LastName = register.LastName,
                    PhoneNumber = register.PhoneNumber,
                    DOB = register.DOB,
                    Email = register.Email



                };




                user.Addresses.Add(new Address { AddressLine = register.AddressLine, Country = register.Country, UserId = user.Id });
                var identityResult = await UserManager.CreateAsync(user, register.Password);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");

                }
                ModelState.AddModelError("", identityResult.Errors.FirstOrDefault());
            }

           
            return View(register);
        }
    }
}