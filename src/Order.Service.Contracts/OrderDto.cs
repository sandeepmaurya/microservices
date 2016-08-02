using System.Collections.Generic;

namespace Order.Service.Contracts
{
    public class OrderDto
    {
        public string Id { get; set; }
        public StoreDto Store { get; set; }
        public CustomerDto Customer { get; set; }
        public AddressDto Address { get; set; }
        public List<LineItemDto> Items { get; set; }
        public string CouponCode { get; set; }
        public double Discount { get; set; }
        public List<TaxLineDto> TaxLines { get; set; }
        public double TotalAmount { get; set; }
    }
}