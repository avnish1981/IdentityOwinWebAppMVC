using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using IdentityOwinWebApp.Demo.EntityFramework;
using IdentityOwinWebApp.Demo.Models;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;
using System.Configuration;
using IdentityOwinWebApp.Demo.Services;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(IdentityOwinWebApp.Demo.Startup))]

namespace IdentityOwinWebApp.Demo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DBPluralsight;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            //app.CreatePerOwinContext(() => new IdentityDbContext(connectionString)); //Connection string setup
            //app.CreatePerOwinContext<UserStore<IdentityUser>>((opt, cont) => new UserStore<IdentityUser>(cont.Get<IdentityDbContext>())); //Connecting TO DB
            //app.CreatePerOwinContext<UserManager<IdentityUser>>((opt, cont) => new UserManager<IdentityUser>(cont.Get<UserStore<IdentityUser>>())); // COnnection To UserManager Class and property


            //app.CreatePerOwinContext<SignInManager<IdentityUser, string>>((opt, cont) => new SignInManager<IdentityUser, string>(cont.Get<UserManager<IdentityUser>>(), cont.Authentication));


            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie


            //});
            // This configuration is required for extending User with additional propties.
            /*  const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DBPluralsightExtendedUser;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
              app.CreatePerOwinContext(() => new ExtendedUserDbContext(connectionString)); // connection string configuration

              app.CreatePerOwinContext<UserStore<ExtendingUser>>((opt, cont) => new UserStore<ExtendingUser>(cont.Get<ExtendedUserDbContext>())); //Connecting To DB for Extending Users


              app.CreatePerOwinContext<UserManager<ExtendingUser>>(

                  //(opt, cont) => new UserManager<ExtendingUser>(cont.Get<UserStore<ExtendingUser>>())); // Connecting to UserManager Class.
                  (opt, cont)=>
                  {
                      var usermanager = new UserManager<ExtendingUser>(cont.Get<UserStore<ExtendingUser>>());
                      usermanager.RegisterTwoFactorProvider("SMS", new PhoneNumberTokenProvider<ExtendingUser, string> {MessageFormat= "Your security code is: {0}" });  // here registring the phone number provider for reciving the code.
                      usermanager.SmsService = new SmsService();
                      usermanager.UserTokenProvider = new DataProtectorTokenProvider<ExtendingUser>(opt.DataProtectionProvider.Create()); // This will generate and validate token for Reset and confirmation email.
                      usermanager.EmailService = new EmailService();
                      usermanager.UserValidator = new UserValidator<ExtendingUser>(usermanager) { RequireUniqueEmail = true, AllowOnlyAlphanumericUserNames = true }; // This code will validate the unique number of users in system
                      usermanager.PasswordValidator = new PasswordValidator // this code will generate policy for password .
                      {
                          RequireDigit = true,
                          RequiredLength = 8,
                          RequireLowercase=true,
                          RequireUppercase=true,
                          RequireNonLetterOrDigit=true 
                      };
                      usermanager.UserLockoutEnabledByDefault = true; // This will enable 1 in database for LockOutEnable field.
                      usermanager.MaxFailedAccessAttemptsBeforeLockout = 2; // This line of code provide 2  attempts and update end date in LockOutEndDateUtc
                      usermanager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(3); // User account will be active after 3 mins.
                      return usermanager;
                  }); 

              app.CreatePerOwinContext<SignInManager<ExtendingUser, string>>((opt, cont) => new SignInManager<ExtendingUser, string>(cont.Get<UserManager<ExtendingUser>>(), cont.Authentication));*/

            app.CreatePerOwinContext<UserManager<ExtendingUser,string>>(()=> DependencyResolver.Current.GetService<UserManager<ExtendingUser,string>>()); // This will register security stamp validator
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = TimeSpan.FromMinutes(20),
                //CookieName = "AvnishCookie"
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity=SecurityStampValidator.OnValidateIdentity<UserManager<ExtendingUser >,ExtendingUser >(
                        validateInterval:TimeSpan.FromSeconds(3),regenerateIdentity:(manager,user)=> manager.CreateIdentityAsync(user,DefaultAuthenticationTypes.ApplicationCookie))
                }

                //This above section describe about if after changing user crendentilas , how system will validate the correct user session identity.

                // For That Application has to hook in to   cookie authentication  middleware. this is done by provider key and here .
                //validateInterval - here security stamp Validator check the userstore to see users security stamp has changed for that in this case we are setting quite low only 3 sec, Pratically it has to be set on 30 min to 1 hour depends upon cookie lifetime.
                //regenerateIdentity -  This function is allow to generate fresh cookie to be issued , if security stamp is not changed
            });
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["google:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["google:ClientSecret"],
                Caption = "Google"


            });

            app.UseFacebookAuthentication(
                new FacebookAuthenticationOptions
                {
                    // Fill in the application ID and secret of your Facebook authentication application
                    AppId = ConfigurationManager.AppSettings["facebook:appid"],
                    AppSecret = ConfigurationManager.AppSettings["facebook:appsecret"],
                    Caption="Facebook"
                });





        }
    }
}
