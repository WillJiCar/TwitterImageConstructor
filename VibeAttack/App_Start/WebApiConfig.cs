using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace VibeAttack.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
            //configuration.EnableCors();
        }
    }
}