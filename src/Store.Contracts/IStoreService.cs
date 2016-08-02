using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Contracts
{
    public interface IStoreService : Microsoft.ServiceFabric.Services.Remoting.IService
    {
        Task<IEnumerable<StoreInfoDto>> GetStoresByCityAsync(int cityId);
        Task<StoreInfoDto> GetStoreByIdAsync(string id);
        Task<IEnumerable<CityInfoDto>> GetCitiesAsync();
        Task AddCityAsync(CityInfoDto cityInfo);
        Task AddStoreAsync(StoreInfoDto storeInfo);
    }
}
