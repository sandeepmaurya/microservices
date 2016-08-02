using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Store.Contracts;

namespace GatewayApi.Controllers
{
    [RoutePrefix("store")]
    public class StoreController : ApiController
    {
        [HttpGet]
        [Route("cities/{cityId}/stores")]
        public async Task<IEnumerable<StoreInfoDto>> GetStoresByCity(int cityId)
        {
            IStoreService proxy = GetProxy(cityId % 10);
            return await proxy.GetStoresByCityAsync(cityId);
        }

        [HttpGet]
        [Route("cities")]
        public async Task<IEnumerable<CityInfoDto>> GetCities()
        {
            List<CityInfoDto> cities = new List<CityInfoDto>();
            // Scatter-gather.
            IEnumerable<IStoreService> proxies = GetAllProxies();
            foreach (var proxy in proxies)
            {
                cities.AddRange(await proxy.GetCitiesAsync());
            }

            return cities;
        }

        private IEnumerable<IStoreService> GetAllProxies()
        {
            List<IStoreService> proxies = new List<IStoreService>();
            for (int i = 0; i < 9; i++)
            {
                proxies.Add(GetProxy(i));
            }

            return proxies;
        }

        private IStoreService GetProxy(int partition)
        {
            return ServiceProxy.Create<IStoreService>(
                new Uri("fabric:/ContosoPizzaApp/StoreService"),
                new ServicePartitionKey(partition));
        }
    }
}
