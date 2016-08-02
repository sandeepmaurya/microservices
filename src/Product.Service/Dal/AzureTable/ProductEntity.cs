using Microsoft.WindowsAzure.Storage.Table;

namespace Product.Service.Dal.AzureTable
{
    public class ProductEntity : TableEntity
    {
        public const string PartitionName = "p";

        // PartitionKey is always 'p'.
        // RowKey is product id.
        public ProductEntity()
        {
            this.PartitionKey = ProductEntity.PartitionName;
        }
        public string Name { get; set; }
        public string ImageUri { get; set; }
    }
}
