using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Order.Service.Contracts
{
    public interface IOrderService : IService
    {
        Task<OrderDto> CreateNewOrder(int cityId, string storeId);
        Task<OrderDto> GetOrder(string id);
        Task<OrderDto> AddItem(string id, CartItemDto input);
        Task<OrderDto> RemoveItem(string id, string itemId);
        Task Checkout(string id);
        Task<OrderDto> UpdateCustomerInfo(string id, CustomerDto customer);
        Task<OrderDto> UpdateAddress(string id, AddressDto address);
        Task<OrderDto> ApplyCoupon(string id, string couponCode);
        Task<OrderDto> PaymentComplete(string id, PaymentDto paymentDetails);
    }
}
