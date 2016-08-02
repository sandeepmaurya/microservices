using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Product.Contracts
{
    public interface IProductService : IService
    {
        Task<IEnumerable<StoreProductDto>> GetStoreProducts(string storeId);

        Task<StoreProductDto> GetStoreProduct(string storeId, string productId);

        Task AddProduct(ProductDto product);

        Task AddStoreProduct(string storeId, string productId, int quantity, double unitPrice);
    }
}
