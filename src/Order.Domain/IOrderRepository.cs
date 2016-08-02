using System.Threading.Tasks;

namespace Order.Domain
{
    public interface IOrderRepository
    {
        Task Add(Order order);
        Task<Order> GetById(string orderId);
        Task Update(Order order);
    }
}
