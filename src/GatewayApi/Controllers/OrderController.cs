using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NLog;
using Order.Service.Contracts;

namespace GatewayApi.Controllers
{
    [RoutePrefix("order")]
    public class OrderController : ApiController
    {
        private ILogger logger;

        public OrderController(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Create new order.
        /// POST /order, body contains storeId.
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> CreateNewOrder(int cityId, string storeId)
        {
            this.logger.Trace("Entering CreateNewOrder...");
            IOrderService proxy = GetOrderServiceProxy();
            OrderDto dto = await proxy.CreateNewOrder(cityId, storeId);
            return Request.CreateResponse<OrderDto>(HttpStatusCode.Created, dto);
        }

        /// <summary>
        /// Get order by id.
        /// GET /order/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public async Task<OrderDto> GetOrder(string id)
        {
            IOrderService proxy = GetOrderServiceProxy();
            return await proxy.GetOrder(id);
        }

        /// <summary>
        /// Add item to order.
        /// POST /order/{id}/items, body contains product id and quantity.
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        [Route("{id}/items")]
        public async Task<OrderDto> AddItem(string id, [FromBody] CartItemDto input)
        {
            IOrderService proxy = GetOrderServiceProxy();
            return await proxy.AddItem(id, input);
        }

        /// <summary>
        /// Remove item from cart.
        /// DELETE /order/{id}/items/{itemId}
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete]
        [Route("{id}/items/{itemId}")]
        public async Task<OrderDto> RemoveItem(string id, string itemId)
        {
            IOrderService proxy = GetOrderServiceProxy();
            return await proxy.RemoveItem(id, itemId);
        }

        /// <summary>
        /// Checkout.
        /// POST /order/{id}/checkout.
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        [Route("{id}/checkout")]
        public async Task<OrderDto> Checkout(string id)
        {
            IOrderService proxy = GetOrderServiceProxy();
            await proxy.Checkout(id);
            return await proxy.GetOrder(id);
        }

        /// <summary>
        /// Update order customer information.
        /// POST /order/{id}/customer
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        [Route("{id}/customer")]
        public async Task<OrderDto> UpdateCustomerDetails(string id, [FromBody] OrderDto order)
        {
            IOrderService proxy = GetOrderServiceProxy();
            await proxy.UpdateCustomerInfo(id, order.Customer);

            order.Address.City = order.Store.City;
            order.Address.State = order.Store.State;
            order.Address.PinCode = order.Store.PinCode;
            return await proxy.UpdateAddress(id, order.Address);
        }

        /// <summary>
        /// Update coupon.
        /// POST /order/{id}/coupon
        /// </summary>
        [HttpPost]
        [Route("{id}/coupon")]
        public async Task<OrderDto> ApplyCoupon(string id, [FromBody] string couponCode)
        {
            IOrderService proxy = GetOrderServiceProxy();
            return await proxy.ApplyCoupon(id, couponCode);
        }

        /// <summary>
        /// Update payment status.
        /// POST /order/{id}/payment
        /// </summary>
        [HttpPost]
        [Route("{id}/payment")]
        public async Task<OrderDto> PaymentComplete(string id, [FromBody] PaymentDto paymentDetails)
        {
            IOrderService proxy = GetOrderServiceProxy();
            return await proxy.PaymentComplete(id, paymentDetails);
        }

        private IOrderService GetOrderServiceProxy()
        {
            return ServiceProxy.Create<IOrderService>(
                new Uri("fabric:/ContosoPizzaApp/OrderService"));
        }
    }
}
