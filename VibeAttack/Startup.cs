using System;
using Microsoft.Owin;
using Owin;
using Tweetinvi;
using Facebook;

[assembly: OwinStartupAttribute(typeof(VibeAttack.Startup))]
namespace VibeAttack
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            TwiiterConfigureAuth();
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
    }
}
