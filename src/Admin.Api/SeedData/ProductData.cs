using System.Collections.Generic;
using Product.Contracts;

namespace Admin.Api.SeedData
{
    public static class ProductData
    {
        public static IEnumerable<ProductDto> Products
        {
            get
            {
                return new List<ProductDto>
                {
                    new ProductDto { Id = "p-001", Name="Product 1", ImageUri="https://30store.blob.core.windows.net/public/1.png" },
                    new ProductDto { Id = "p-002", Name="Product 2", ImageUri="https://30store.blob.core.windows.net/public/2.png" },
                };
            }
        }

        public static IEnumerable<StoreProductDto> StoreProducts
        {
            get
            {
                return new List<StoreProductDto>
                {
                    new StoreProductDto { StoreId = "10-001", Price = 150.45, Quantity = 101, Product = new ProductDto { Id = "p-001"} },
                    new StoreProductDto { StoreId = "10-001", Price = 180.45, Quantity = 101, Product = new ProductDto { Id = "p-002"} },

                    new StoreProductDto { StoreId = "10-002", Price = 150.45, Quantity = 101, Product = new ProductDto { Id = "p-001"} },
                    new StoreProductDto { StoreId = "10-002", Price = 180.45, Quantity = 101, Product = new ProductDto { Id = "p-002"} },

                    new StoreProductDto { StoreId = "11-001", Price = 150.45, Quantity = 101, Product = new ProductDto { Id = "p-001"} },
                    new StoreProductDto { StoreId = "11-001", Price = 180.45, Quantity = 101, Product = new ProductDto { Id = "p-002"} },

                    new StoreProductDto { StoreId = "11-002", Price = 150.45, Quantity = 101, Product = new ProductDto { Id = "p-001"} },
                    new StoreProductDto { StoreId = "11-002", Price = 180.45, Quantity = 101, Product = new ProductDto { Id = "p-002"} },

                    new StoreProductDto { StoreId = "12-001", Price = 150.45, Quantity = 101, Product = new ProductDto { Id = "p-001"} },
                    new StoreProductDto { StoreId = "12-001", Price = 180.45, Quantity = 101, Product = new ProductDto { Id = "p-002"} },

                    new StoreProductDto { StoreId = "12-002", Price = 150.45, Quantity = 101, Product = new ProductDto { Id = "p-001"} },
                    new StoreProductDto { StoreId = "12-002", Price = 180.45, Quantity = 101, Product = new ProductDto { Id = "p-002"} },

                    new StoreProductDto { StoreId = "13-001", Price = 150.45, Quantity = 101, Product = new ProductDto { Id = "p-001"} },
                    new StoreProductDto { StoreId = "13-001", Price = 170.45, Quantity = 101, Product = new ProductDto { Id = "p-002"} },

                    new StoreProductDto { StoreId = "13-002", Price = 150.45, Quantity = 101, Product = new ProductDto { Id = "p-001"} },
                    new StoreProductDto { StoreId = "13-002", Price = 170.45, Quantity = 101, Product = new ProductDto { Id = "p-002"} },
                };
            }
        }
    }
}
