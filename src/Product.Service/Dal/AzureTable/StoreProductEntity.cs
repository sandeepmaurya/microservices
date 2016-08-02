using Microsoft.WindowsAzure.Storage.Table;

namespace Product.Service.Dal.AzureTable
{
    public class StoreProductEntity : TableEntity
    {
        // PartitionKey is store id.
        // RowKey is product id.
        public int AvailableQuantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
