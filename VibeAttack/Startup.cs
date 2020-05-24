using System;
using Microsoft.Owin.Cors;
using Microsoft.Owin;
using Owin;
using Tweetinvi;
using Microsoft.Extensions.DependencyInjection;
using System.Web.Mvc;
using VibeAttack.App_Start;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(VibeAttack.Startup))]
namespace VibeAttack
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ConfigureAuth(app);
            var resolver = new DefaultDependencyResolver(services.BuildServiceProvider());
            DependencyResolver.SetResolver(resolver);
            TwiiterConfigureAuth();
            app.UseCors(CorsOptions.AllowAll);
        }

        private void TwiiterConfigureAuth()
        {
            var appCreds = Auth.SetApplicationOnlyCredentials("9nONMiLO2V37hIYL1v2vm1sKh",
                "gozRSNleNvrHyPcgYGQMdBWDX5kJl0HW4PhsbqhwiL36aqhzu2", true);

            Auth.InitializeApplicationOnlyCredentials(appCreds);
        }

        private void FacebookConfigureAuth()
        {
            
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersAsServices(typeof(Startup).Assembly.GetExportedTypes()
           .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
           .Where(t => typeof(IController).IsAssignableFrom(t) || t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)));

        }

        /*
         * One important thing to be aware of is that we haven’t completely 
         * switched over to using a DI framework. If you created your project 
         * using the “Individual User Accounts” authentication option, you’ll 
         * also have a Startup.Auth.cs file in the App_Start folder of your 
         * project. That’s where the ConfigureAuth method can be found. If you 
         * haven’t made any changes to that code, the ConfigureAuth method
         * registers the ApplicationDbContext, ApplicationUserManager, and 
         * ApplicationSignInManager with the IAppBuilder instance. More 
         * specifically, it registers them as part of the OwinContext that 
         * is then later retrieved by calling 
         * HttpContext.GetOwinContext().Get<ApplicationSignInManager>() or
         * HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(),
         * which aren’t resolved through the DependencyResolver. 
         * I’ll show how to move these to use our new service provider
         * in an upcoming post.
         https://scottdorman.blog/2016/03/17/integrating-asp-net-core-dependency-injection-in-mvc-4/ */
    }
}
