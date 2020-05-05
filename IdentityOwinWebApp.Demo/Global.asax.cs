using IdentityOwinWebApp.Demo.EntityFramework;
using IdentityOwinWebApp.Demo.Models;
using IdentityOwinWebApp.Demo.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IdentityOwinWebApp.Demo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ConfigureContainer();
        }

        public static  void ConfigureContainer()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            const string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DBPluralsightExtendedUser;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            container.Register(() => new ExtendedUserDbContext(connectionString),Lifestyle.Scoped ); //Passing connection sring to Entity framework
            container.Register(() => new UserStore<ExtendingUser>(container.GetInstance<ExtendedUserDbContext>()), Lifestyle.Scoped); // registring User Store with DI Container
            //REgistring User Manger in DI Container
            container.Register(() =>
            {

                var usermanager = new UserManager<ExtendingUser,string >(container.GetInstance<UserStore<ExtendingUser>>());
                usermanager.RegisterTwoFactorProvider("SMS", new PhoneNumberTokenProvider<ExtendingUser, string> { MessageFormat = "Your security code is: {0}" });  // here registring the phone number provider for reciving the code.
                usermanager.SmsService = new SmsService();
               
                usermanager.EmailService = new EmailService();
                usermanager.UserValidator = new UserValidator<ExtendingUser>(usermanager) { RequireUniqueEmail = true, AllowOnlyAlphanumericUserNames = true }; // This code will validate the unique number of users in system
                usermanager.PasswordValidator = new PasswordValidator // this code will generate policy for password .
                {
                    RequireDigit = true,
                    RequiredLength = 8,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequireNonLetterOrDigit = true
                };
                usermanager.UserLockoutEnabledByDefault = true; // This will enable 1 in database for LockOutEnable field.
                usermanager.MaxFailedAccessAttemptsBeforeLockout = 5; // This line of code provide 2  attempts and update end date in LockOutEndDateUtc
                usermanager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(15); // User account will be active after 15 mins.
                return usermanager;

            },Lifestyle.Scoped );
            // Registering Signin manger with DI Container
            container.Register<SignInManager<ExtendingUser, string>>(Lifestyle.Scoped );
            container.Register(() => container.IsVerifying ? new OwinContext().Authentication :HttpContext.Current.GetOwinContext().Authentication , Lifestyle.Scoped);
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.Verify();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));


        }
    }
}
