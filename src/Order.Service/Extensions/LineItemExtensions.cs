using Order.Domain;
using Order.Service.Contracts;

namespace Order.Service.Extensions
{
    public static class LineItemExtensions
    {
        public static LineItemDto ToDto(this LineItem lineItem)
        {
            return new LineItemDto
            {
                Id = lineItem.Id,
                ProductId = lineItem.Product.Id,
                Quantity = lineItem.Quantity,
                Name = lineItem.Product.Name,
                Description = lineItem.Product.Description,
                UnitPrice = lineItem.Product.UnitPrice,
                ImageUri = lineItem.Product.ImageUri,
                Discount = lineItem.Discount,
                Amount = lineItem.Amount,
            };
        }
    }
}
