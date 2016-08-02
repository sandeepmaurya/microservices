using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using Common.Configuration.ServiceFabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace GatewayApi
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class GatewayApi : StatelessService
    {
        private Common.Configuration.IConfiguration config;

        public GatewayApi(StatelessServiceContext context)
            : base(context)
        {

            ICodePackageActivationContext codePackageContext = this.Context.CodePackageActivationContext;
            ConfigurationPackage configPackage = codePackageContext.GetConfigurationPackageObject("Config");
            ConfigurationSection configSection = configPackage.Settings.Sections["GatewayConfig"];
            this.config = new ServiceConfiguration(configSection);
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            Startup startup = new Startup(this.config);

            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext => new OwinCommunicationListener(
                    startup.Configuration,
                    serviceContext,
                    ServiceEventSource.Current,
                    "ServiceEndpoint"))
            };
        }
    }
}
