using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Product.Contracts;

namespace Product.Service.Dal.AzureTable
{
    public class Storage : IStorage
    {
        private const string ProductTableName = "products";
        private const string StoreProductsTableName = "storeproducts";
        private string storageConnectionString;

        public Storage(IConfiguration config)
        {
            this.storageConnectionString = config.GetValue("StorageConnectionString");
        }

        public void Initialize()
        {
            CloudTable table = GetTableReference(ProductTableName);
            table.CreateIfNotExists();
            table = GetTableReference(StoreProductsTableName);
            table.CreateIfNotExists();
        }

        public async Task AddProduct(ProductDto product)
        {
            CloudTable table = GetTableReference(ProductTableName);
            ProductEntity entity = new ProductEntity
            {
                RowKey = product.Id,
                Name = product.Name,
                ImageUri = product.ImageUri
            };

            TableOperation insertOperation = TableOperation.InsertOrReplace(entity);
            await table.ExecuteAsync(insertOperation);
        }

        public async Task AddStoreProduct(string storeId, string productId, int quantity, double unitPrice)
        {
            CloudTable table = GetTableReference(StoreProductsTableName);
            StoreProductEntity entity = new StoreProductEntity
            {
                PartitionKey = storeId,
                RowKey = productId,
                AvailableQuantity = quantity,
                UnitPrice = unitPrice
            };

            TableOperation insertOperation = TableOperation.InsertOrReplace(entity);
            await table.ExecuteAsync(insertOperation);
        }

        public async Task<StoreProductDto> GetStoreProduct(string storeId, string productId)
        {
            CloudTable table = GetTableReference(StoreProductsTableName);
            TableOperation retrieveOperation = TableOperation.Retrieve<StoreProductEntity>(storeId, productId);
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            if (result.Result == null)
            {
                return null;
            }

            StoreProductEntity storeProductEntity = (StoreProductEntity)result.Result;

            table = GetTableReference(ProductTableName);
            retrieveOperation = TableOperation.Retrieve<ProductEntity>(ProductEntity.PartitionName, productId);
            result = await table.ExecuteAsync(retrieveOperation);
            if (result.Result == null)
            {
                return null;
            }

            ProductEntity productEntity = (ProductEntity)result.Result;
            ProductDto product = GetProductFromEntity(productEntity);
            return GetStoreProductFromEntity(storeProductEntity, product);
        }

        public async Task<IEnumerable<StoreProductDto>> GetStoreProducts(string storeId)
        {
            List<StoreProductDto> storeProducts = new List<StoreProductDto>();
            CloudTable table = GetTableReference(StoreProductsTableName);
            TableQuery<StoreProductEntity> query = new TableQuery<StoreProductEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, storeId));

            Dictionary<string, StoreProductEntity> storeProductEntities = new Dictionary<string, StoreProductEntity>();
            List<string> filterConditions = new List<string>();
            foreach (StoreProductEntity entity in table.ExecuteQuery<StoreProductEntity>(query))
            {
                storeProductEntities.Add(entity.RowKey, entity);
                filterConditions.Add(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, entity.RowKey));
            }

            if (storeProductEntities.Count == 0)
            {
                return storeProducts;
            }

            var combinedFilter = filterConditions[0];
            for (int i = 1; i < filterConditions.Count; i++)
            {
                combinedFilter = TableQuery.CombineFilters(combinedFilter, TableOperators.Or, filterConditions[i]);
            }

            table = GetTableReference(ProductTableName);
            TableQuery<ProductEntity> productQuery = new TableQuery<ProductEntity>().Where(combinedFilter);
            foreach (ProductEntity entity in table.ExecuteQuery<ProductEntity>(productQuery))
            {
                ProductDto product = GetProductFromEntity(entity);
                StoreProductEntity storeProductEntity = storeProductEntities[entity.RowKey];
                storeProducts.Add(GetStoreProductFromEntity(storeProductEntity, product));
            }

            return storeProducts;
        }

        private CloudTable GetTableReference(string name)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(name);
            return table;
        }

        private StoreProductDto GetStoreProductFromEntity(StoreProductEntity storeProductEntity, ProductDto product)
        {
            return new StoreProductDto
            {
                StoreId = storeProductEntity.PartitionKey,
                Product = product,
                Price = storeProductEntity.UnitPrice,
                Quantity = storeProductEntity.AvailableQuantity
            };
        }

        private ProductDto GetProductFromEntity(ProductEntity productEntity)
        {
            return new ProductDto
            {
                Id = productEntity.RowKey,
                Name = productEntity.Name,
                ImageUri = productEntity.ImageUri
            };
        }
    }
}
