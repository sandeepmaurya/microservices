using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Threading;
using System.Threading.Tasks;
using Common.Configuration;
using Common.Configuration.ServiceFabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Product.Contracts;
using Product.Service.Dal;
using Product.Service.Dal.AzureTable;

namespace Product.Service
{
    internal sealed class Service : StatelessService, IProductService
    {
        private IStorage storage;

        public Service(StatelessServiceContext context)
            : base(context)
        {
            ICodePackageActivationContext codePackageContext = this.Context.CodePackageActivationContext;
            ConfigurationPackage configPackage = codePackageContext.GetConfigurationPackageObject("Config");
            ConfigurationSection configSection = configPackage.Settings.Sections["ProductServiceConfig"];

            // TODO: Inject concrete implementations.
            IConfiguration config = new ServiceConfiguration(configSection);
            Storage tableStorage = new Storage(config);
            tableStorage.Initialize();
            this.storage = tableStorage;
        }

        public async Task AddProduct(ProductDto product)
        {
            await this.storage.AddProduct(product);
        }

        public async Task AddStoreProduct(string storeId, string productId, int quantity, double unitPrice)
        {
            await this.storage.AddStoreProduct(storeId, productId, quantity, unitPrice);
        }

        public async Task<StoreProductDto> GetStoreProduct(string storeId, string productId)
        {
            return await this.storage.GetStoreProduct(storeId, productId);
        }

        public async Task<IEnumerable<StoreProductDto>> GetStoreProducts(string storeId)
        {
            return await this.storage.GetStoreProducts(storeId);
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener(context =>
            this.CreateServiceRemotingListener(context)) };
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
