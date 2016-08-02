using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Store.Contracts;

namespace Store.Service
{
    internal sealed class StoreService : StatefulService, IStoreService
    {
        public StoreService(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<IEnumerable<StoreInfoDto>> GetStoresByCityAsync(int cityId)
        {
            IReliableDictionary<int, List<StoreInfoDto>> storesByCityDictionary = await GetStoresByCityDictionaryAsync();
            using (var tx = this.StateManager.CreateTransaction())
            {
                var stores = await storesByCityDictionary.TryGetValueAsync(tx, cityId);
                return stores.HasValue ? stores.Value : new List<StoreInfoDto>();
            }
        }

        public async Task<StoreInfoDto> GetStoreByIdAsync(string id)
        {
            IReliableDictionary<string, StoreInfoDto> stores = await GetStoresDictionaryAsync();
            using (var tx = this.StateManager.CreateTransaction())
            {
                var store = await stores.TryGetValueAsync(tx, id);
                return store.HasValue ? store.Value : null;
            }
        }

        public async Task<IEnumerable<CityInfoDto>> GetCitiesAsync()
        {
            List<CityInfoDto> cities = new List<CityInfoDto>();
            IReliableDictionary<int, CityInfoDto> citiesDictionary = await GetCitiesDictionaryAsync();
            using (var tx = this.StateManager.CreateTransaction())
            {
                var enumerable = await citiesDictionary.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(new CancellationToken()))
                {
                    cities.Add(enumerator.Current.Value);
                }

                return cities;
            }
        }

        public async Task AddCityAsync(CityInfoDto cityInfo)
        {
            IReliableDictionary<int, CityInfoDto> citiesDictionary = await GetCitiesDictionaryAsync();
            using (var tx = this.StateManager.CreateTransaction())
            {
                await citiesDictionary.AddOrUpdateAsync(tx, cityInfo.Id, cityInfo, (key, value) => cityInfo);
                await tx.CommitAsync();
            }
        }

        public async Task AddStoreAsync(StoreInfoDto storeInfo)
        {
            IReliableDictionary<string, StoreInfoDto> storesDictionary = await GetStoresDictionaryAsync();
            IReliableDictionary<int, List<StoreInfoDto>> storesByCityDictionary = await GetStoresByCityDictionaryAsync();
            using (var tx = this.StateManager.CreateTransaction())
            {
                await storesDictionary.AddOrUpdateAsync(tx, storeInfo.StoreId, storeInfo, (key, value) => storeInfo);

                await storesByCityDictionary.AddOrUpdateAsync(
                    tx,
                    storeInfo.CityId,
                    new List<StoreInfoDto> { storeInfo },
                    (key, prev) =>
                    {
                        if (!prev.Any(s => s.StoreId == storeInfo.StoreId))
                        {
                            prev.Add(storeInfo);
                        }

                        return prev;
                    });

                await tx.CommitAsync();
            }

        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context))
            };
        }

        private async Task<IReliableDictionary<int, CityInfoDto>> GetCitiesDictionaryAsync()
        {
            return await this.StateManager.GetOrAddAsync<IReliableDictionary<int, CityInfoDto>>("cities");
        }

        private async Task<IReliableDictionary<int, List<StoreInfoDto>>> GetStoresByCityDictionaryAsync()
        {
            return await this.StateManager.GetOrAddAsync<IReliableDictionary<int, List<StoreInfoDto>>>("stores-by-city");
        }

        private async Task<IReliableDictionary<string, StoreInfoDto>> GetStoresDictionaryAsync()
        {
            return await this.StateManager.GetOrAddAsync<IReliableDictionary<string, StoreInfoDto>>("stores");
        }
    }
}
