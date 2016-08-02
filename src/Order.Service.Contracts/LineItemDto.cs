namespace Order.Service.Contracts
{
    public class LineItemDto
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImageUri { get; set; }
        public double Discount { get; set; }
        public double Amount { get; set; }
    }
}