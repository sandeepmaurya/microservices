using System.Collections.Generic;
using System.Threading.Tasks;
using Product.Contracts;

namespace Product.Service.Dal
{
    public interface IStorage
    {
        Task<IEnumerable<StoreProductDto>> GetStoreProducts(string storeId);

        Task<StoreProductDto> GetStoreProduct(string storeId, string productId);

        Task AddProduct(ProductDto product);

        Task AddStoreProduct(string storeId, string productId, int quantity, double unitPrice);
    }
}
