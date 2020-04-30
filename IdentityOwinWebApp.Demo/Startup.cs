using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using IdentityOwinWebApp.Demo.EntityFramework;
using IdentityOwinWebApp.Demo.Models;
using Microsoft.Owin.Security.Google;
using System.Configuration;

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
            const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DBPluralsightExtendedUser;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            app.CreatePerOwinContext(() => new ExtendedUserDbContext(connectionString)); // connection string configuration

            app.CreatePerOwinContext<UserStore<ExtendingUser>>((opt, cont) => new UserStore<ExtendingUser>(cont.Get<ExtendedUserDbContext>())); //Connecting To DB.

            app.CreatePerOwinContext<UserManager<ExtendingUser>>(

                //(opt, cont) => new UserManager<ExtendingUser>(cont.Get<UserStore<ExtendingUser>>())); // Connecting to UserManager Class.
                (opt, cont)=>
                {
                    var usermanager = new UserManager<ExtendingUser>(cont.Get<UserStore<ExtendingUser>>());
                    usermanager.RegisterTwoFactorProvider("SMS", new PhoneNumberTokenProvider<ExtendingUser> {MessageFormat="Token : {0}" });  // here registring the phone number provider for reciving the code.
                    usermanager.SmsService = new SmsService();
                    return usermanager;
                });

            app.CreatePerOwinContext<SignInManager<ExtendingUser, string>>((opt, cont) => new SignInManager<ExtendingUser, string>(cont.Get<UserManager<ExtendingUser>>(), cont.Authentication));
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie

            });
            app.UseTwoFactorSignInCookie (DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["google:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["google:ClientSecret"],
                Caption = "Google"
                

            });
            

            


        }
    }
}
