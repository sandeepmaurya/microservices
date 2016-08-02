using System.Web.Http;
using System.Web.Http.Cors;
using Common.Configuration;
using Microsoft.Practices.Unity;
using NLog;
using Owin;

namespace GatewayApi
{
    public class Startup
    {
        private IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config;
        }

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            // Enable attribute routing.
            config.MapHttpAttributeRoutes();

            // Setup DI.
            var container = new UnityContainer();
            ILogger logger = LogManager.GetLogger("GatewayApiLogger");
            container.RegisterInstance<ILogger>(logger);
            config.DependencyResolver = new UnityResolver(container);

            // Enable Cors.
            var corsOrigins = this.config.GetValue("CorsOrigins");
            var corsProvider = new EnableCorsAttribute(corsOrigins, "*", "*");
            config.EnableCors(corsProvider);

            appBuilder.UseWebApi(config);
        }
    }
}
