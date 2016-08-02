using System.Runtime.Serialization;

namespace Product.Contracts
{
    [DataContract]
    public sealed class ProductDto
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ImageUri { get; set; }
    }
}
