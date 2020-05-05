using IdentityOwinWebApp.Demo.Models;
using Microsoft.AspNet.Identity;

using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.Owin.Logging;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IdentityOwinWebApp.Demo.Controllers
{
    public class AccountController : Controller
    {
        public UserManager<ExtendingUser,string > UserManager;
        public SignInManager<ExtendingUser , string> SignInManager;

        //public UserManager<IdentityUser> UserManager => HttpContext.GetOwinContext().Get<UserManager<IdentityUser>>();
        //public SignInManager<IdentityUser, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<IdentityUser, string>>();
        //Extending User Configuration
        //public UserManager<ExtendingUser> UserManager => HttpContext.GetOwinContext().Get<UserManager<ExtendingUser>>();
        //public SignInManager<ExtendingUser, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<ExtendingUser, string>>();
        // GET: Account
        public AccountController(UserManager<ExtendingUser,string> UserManager, SignInManager<ExtendingUser, string> SignInManager)
        {
            this.UserManager = UserManager;
            this.SignInManager = SignInManager;
        }

        /// <summary>
        /// This Method will redirect to External Login page of Google and etc identity provider system.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExternalLoginAuthentication(string provider)
        {
            SignInManager.AuthenticationManager.Challenge(new AuthenticationProperties

            {
                RedirectUri = Url.Action("ExternalLoginCallback", new { provider })

            }, provider);
            return new HttpUnauthorizedResult();
        }

        public async Task<ActionResult> ExternalLoginCallback(string provider)
        {
            var loginInfo = await SignInManager.AuthenticationManager.GetExternalLoginInfoAsync();
            var signInStatus = await SignInManager.ExternalSignInAsync (loginInfo , false);

            switch (signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                default:
                    var user = await UserManager.FindByEmailAsync(loginInfo.ExternalIdentity.FindFirstValue(ClaimTypes.Email  )); // checking here user has local account in the ASPNetUsers Table
                    
                    //creating new user in ASPNETUsers table 
                    if(user == null)
                    {                       
                        user = new ExtendingUser
                        {
                            UserName  = loginInfo.ExternalIdentity.FindFirstValue(ClaimTypes.GivenName ),
                            Email = loginInfo.ExternalIdentity.FindFirstValue(ClaimTypes.Email),
                            TwoFactorEnabled = true
                        };
                        await UserManager.CreateAsync(user); //This will create new record in AspnetUsers table
                        await UserManager.AddLoginAsync(user.Id, loginInfo.Login); // This will create new Record in AspnetUsersLogin Table for external Authentication.
                    }
                    else if (user.Email != null)
                    {
                        await UserManager.AddLoginAsync(user.Id, loginInfo.Login); // This will create new Record in AspnetUsersLogin Table for external Authentication.
                    }

                    return RedirectToAction("Index", "Home");
            }

        }
      


            public ActionResult Login()
        {
            return View();
        }
        [HttpPost ]
        public async Task<ActionResult> Login(Login login )
        {
            
           var   signInStatus =  await SignInManager.PasswordSignInAsync(login.UserName, login.Password,true,true);
            switch(signInStatus)
            {
                case SignInStatus.Success:
                     return RedirectToAction("Index", "Home");
                   // return RedirectToAction("ChooseProvider");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("ChooseProvider");
                                   
                default:
                    ModelState.AddModelError("", "Invalid Crendentials");
                    return View(login);
            }
            signinresult
         
            
        }
        [HttpGet ]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPassword model )
        {
            var user = await UserManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var resetUrl = Url.Action("PasswordReset", "Account", new { userid = user.Id, token = token }, Request.Url.Scheme);
             
                await UserManager.SendEmailAsync(user.Id, "Password Reset", $"Use link to reset password: {resetUrl}");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet ]
        public ActionResult PasswordReset(string userId,string token)
        {
            return View(new PasswordReset { UserId = userId, Token = token });
        }
        [HttpPost ]
        public async Task<ActionResult > PasswordReset (PasswordReset reset )
        {
           var identityResult =  await UserManager.ResetPasswordAsync(reset.UserId, reset.Token, reset.Password);
             if(identityResult == null)
            {
                return View("Error");
            }
            return RedirectToAction("Index", "Home");   

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
            var signInStatus = await SignInManager.TwoFactorSignInAsync (two.Provider, two.Code,true,two.RememberBrowser);
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
        public async Task<ActionResult> Register(ExtendingUser  register)
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

                    UserName = register.Email,
                    LastName = register.LastName,
                    PhoneNumber = register.PhoneNumber,
                    DOB = register.DOB,
                    Email = register.Email,
                    FirstName = register.FirstName ,
                    Country = register.Country 
                   
                    
                    
                    

              };
                user.Addresses.Add(new Address { AddressLine = register.AddressesLine, Country = register.Country, UserId = user.Id });
                var identityResult = await UserManager.CreateAsync(user, register.PasswordHash);//it will store record in aspnetUsers table
                if (identityResult.Succeeded)
                {
                    var token = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var confirmUrl = Url.Action("ConfirmEmail", "Account", new { userid = user.Id, token = token },Request.Url.Scheme );
                    await UserManager.SendEmailAsync(user.Id, "Email Confirmation", $"Use link to confirm Email: {confirmUrl }");
                    return RedirectToAction("Index", "Home");

                }
                ModelState.AddModelError("", identityResult.Errors.FirstOrDefault());
            }
          
            return View(register);
        }

        public async Task< ActionResult> ConfirmEmail(string userid,string token)
        {
            var identityResult = await UserManager.ConfirmEmailAsync(userid, token);
            if(!identityResult.Succeeded  )
            {
                return View("Error");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}