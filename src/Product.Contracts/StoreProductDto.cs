using System.Runtime.Serialization;

namespace Product.Contracts
{
    [DataContract]
    public class StoreProductDto
    {
        [DataMember]
        public string StoreId { get; set; }

        [DataMember]
        public ProductDto Product { get; set; }

        [DataMember]
        public double Price { get; set; }

        [DataMember]
        public int Quantity { get; set; }
    }
}
