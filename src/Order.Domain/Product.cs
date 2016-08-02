namespace Order.Domain
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double UnitPrice { get; set; }
        public string ImageUri { get; set; }

        public Product(string id, string name, string description, double unitPrice, string imageUri)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.UnitPrice = unitPrice;
            this.ImageUri = imageUri;
        }
    }
}
