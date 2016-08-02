using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Threading;
using System.Threading.Tasks;
using Common.Configuration.ServiceFabric;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using NLog;
using Order.Domain;
using Order.Domain.Coupon;
using Order.Domain.Tax;
using Order.Infrastructure.AzureTable;
using Order.Infrastructure.DocDb;
using Order.Infrastructure.Factories;
using Order.Service.Contracts;
using Order.Service.Extensions;
using Product.Contracts;
using Store.Contracts;

namespace Order.Service
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Service : StatelessService, IOrderService
    {
        private IOrderRepository orderRepository;
        private ICouponRepository couponRepository;
        private ITaxFactory taxFactory;
        private ILogger logger;

        public Service(StatelessServiceContext context)
            : base(context)
        {
            ICodePackageActivationContext codePackageContext = this.Context.CodePackageActivationContext;
            ConfigurationPackage configPackage = codePackageContext.GetConfigurationPackageObject("Config");
            ConfigurationSection configSection = configPackage.Settings.Sections["OrderServiceConfig"];

            // TODO: Inject concrete implementations.
            Common.Configuration.IConfiguration config = new ServiceConfiguration(configSection);
            this.orderRepository = new OrderRepository(config);
            this.couponRepository = new CouponRepository();
            this.taxFactory = new TaxFactoryNoServiceCharge();
            this.logger = LogManager.GetCurrentClassLogger();
            OrderRepository.Initialize(config);
        }

        public async Task<OrderDto> AddItem(string id, CartItemDto input)
        {
            this.logger.Trace<LogMessage>(new LogMessage
            {
                CorrelationId = Guid.NewGuid().ToString(),
                EventId = 10000,
                Payload = new Dictionary<string, object>
                {
                    { "id", id},
                    { "input", input }
                }
            });

            Domain.Order order = await this.orderRepository.GetById(id);

            // Anticorruption: Translating DTOs from other services to our contracts.
            IProductService productProxy = GetProductServiceProxy();
            var externalDto = await productProxy.GetStoreProduct(order.Store.Id, input.ProductId);
            Order.Domain.Product product = new Domain.Product(
                externalDto.Product.Id,
                externalDto.Product.Name,
                null,
                (double)externalDto.Price,
                externalDto.Product.ImageUri);

            LineItem lineItem = new LineItem(Guid.NewGuid().ToString(), product, input.Quantity);
            order.AddLineItem(lineItem);
            await this.orderRepository.Update(order);
            return order.ToDto();
        }

        public async Task<OrderDto> ApplyCoupon(string id, string couponCode)
        {
            ICoupon coupon = this.couponRepository.GetByCode(couponCode);
            Domain.Order order = await this.orderRepository.GetById(id);
            order.ApplyCoupon(coupon);
            await this.orderRepository.Update(order);
            return order.ToDto();
        }

        public async Task Checkout(string id)
        {
            Domain.Order order = await this.orderRepository.GetById(id);
            order.Checkout();
            await this.orderRepository.Update(order);
        }

        public async Task<OrderDto> CreateNewOrder(int cityId, string storeId)
        {
            // Anticorruption: Translating DTOs from other services to our contracts.
            IStoreService storeProxy = GetStoreServiceProxy(cityId % 10);
            var externalDto = await storeProxy.GetStoreByIdAsync(storeId);
            Domain.Store store = new Domain.Store
            {
                Id = externalDto.StoreId,
                AddressLine1 = externalDto.Name,
                AddressLine2 = externalDto.Address,
                City = externalDto.CityName,
                State = externalDto.State,
                PinCode = externalDto.PinCode
            };

            Domain.Order order = new Domain.Order(Guid.NewGuid().ToString(), store, this.taxFactory.GetRootStep());
            await this.orderRepository.Add(order);
            return order.ToDto();
        }

        public async Task<OrderDto> GetOrder(string id)
        {
            Domain.Order order = await this.orderRepository.GetById(id);
            return order.ToDto();
        }

        public async Task<OrderDto> PaymentComplete(string id, PaymentDto paymentDetails)
        {
            Domain.Order order = await this.orderRepository.GetById(id);
            order.PaymentComplete(paymentDetails.BankName, paymentDetails.TransactionId);
            await this.orderRepository.Update(order);
            return order.ToDto();
        }

        public async Task<OrderDto> RemoveItem(string id, string itemId)
        {
            Domain.Order order = await this.orderRepository.GetById(id);
            order.RemoveLineItem(itemId);
            await this.orderRepository.Update(order);
            return order.ToDto();
        }

        public async Task<OrderDto> UpdateAddress(string id, AddressDto address)
        {
            Domain.Order order = await this.orderRepository.GetById(id);
            order.SetDeliveryAddress(address.AddressLine1, address.AddressLine2, address.City, address.State, address.PinCode);
            await this.orderRepository.Update(order);
            return order.ToDto();
        }

        public async Task<OrderDto> UpdateCustomerInfo(string id, CustomerDto customer)
        {
            Domain.Order order = await this.orderRepository.GetById(id);
            order.SetCustomerDetails(customer.FirstName, customer.LastName, customer.Email, customer.Mobile);
            await this.orderRepository.Update(order);
            return order.ToDto();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] { new ServiceInstanceListener(context => this.CreateServiceRemotingListener(context)) };
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(TimeSpan.FromSeconds(100), cancellationToken);
            }
        }

        private IProductService GetProductServiceProxy()
        {
            return ServiceProxy.Create<IProductService>(
                new Uri("fabric:/ContosoPizzaApp/ProductService"));
        }

        private IStoreService GetStoreServiceProxy(int partition)
        {
            return ServiceProxy.Create<IStoreService>(
                new Uri("fabric:/ContosoPizzaApp/StoreService"),
                new ServicePartitionKey(partition));
        }
    }
}
