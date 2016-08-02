using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Product.Contracts;

namespace GatewayApi.Controllers
{
    [RoutePrefix("product")]
    public class ProductController : ApiController
    {
        [HttpGet]
        [Route("stores/{storeId}/products")]
        public async Task<IEnumerable<StoreProductDto>> GetStoreProducts(string storeId)
        {
            IProductService proxy = GetServiceProxy();
            return await proxy.GetStoreProducts(storeId);
        }

        [HttpGet]
        [Route("stores/{storeId}/products/{productId}")]
        public async Task<StoreProductDto> GetStoreProduct(string storeId, string productId)
        {
            IProductService proxy = GetServiceProxy();
            return await proxy.GetStoreProduct(storeId, productId);
        }

        private IProductService GetServiceProxy()
        {
            return ServiceProxy.Create<IProductService>(
                new Uri("fabric:/ContosoPizzaApp/ProductService"));
        }
    }
}
