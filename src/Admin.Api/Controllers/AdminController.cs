using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Admin.Api.SeedData;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Product.Contracts;
using Store.Contracts;

namespace Admin.Api.Controllers
{
    [RoutePrefix("admin")]
    public class AdminController : ApiController
    {
        [HttpPost]
        [Route("seed/store")]
        public async Task<HttpResponseMessage> SeedStore()
        {
            int maxRetries = 5;
            while (true)
            {
                try
                {
                    // Load seed data in store.
                    await PopulateCitiesAsync();
                    await PopulateStoresAsync();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch
                {
                    if (maxRetries-- <= 0)
                    {
                        throw;
                    }

                    // TODO: Log.
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }
            }
        }


        [HttpPost]
        [Route("seed/product")]
        public async Task<HttpResponseMessage> AddProduct([FromBody]ProductDto product)
        {
            int maxRetries = 5;
            while (true)
            {
                try
                {
                    await PopulateProductsAsync();
                    await PopulateStoreProductsAsync();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch
                {
                    if (maxRetries-- <= 0)
                    {
                        throw;
                    }
                    await Task.Delay(TimeSpan.FromSeconds(3));
                }
            }
        }

        private async Task PopulateStoreProductsAsync()
        {
            var proxy = GetProductProxy();
            foreach (var storeProduct in ProductData.StoreProducts)
            {
                await proxy.AddStoreProduct(
                    storeProduct.StoreId,
                    storeProduct.Product.Id,
                    storeProduct.Quantity,
                    storeProduct.Price);
            }
        }

        private async Task PopulateProductsAsync()
        {
            var proxy = GetProductProxy();
            foreach (var product in ProductData.Products)
            {
                await proxy.AddProduct(product);
            }
        }

        private async Task PopulateStoresAsync()
        {
            foreach (var store in StoreData.Stores)
            {
                var proxy = GetStoreProxy(store.CityId % 10);
                await proxy.AddStoreAsync(store);
            }
        }

        private async Task PopulateCitiesAsync()
        {
            foreach (var city in StoreData.Cities)
            {
                var proxy = GetStoreProxy(city.Id % 10);
                await proxy.AddCityAsync(city);
            }
        }

        private IStoreService GetStoreProxy(int partition)
        {
            return ServiceProxy.Create<IStoreService>(
                new Uri("fabric:/ContosoPizzaApp/StoreService"),
                new ServicePartitionKey(partition));
        }

        private IProductService GetProductProxy()
        {
            return ServiceProxy.Create<IProductService>(
                new Uri("fabric:/ContosoPizzaApp/ProductService"));
        }
    }
}
