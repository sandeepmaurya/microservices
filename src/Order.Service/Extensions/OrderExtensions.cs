using System;
using System.Collections.Generic;
using Order.Domain;
using Order.Service.Contracts;

namespace Order.Service.Extensions
{
    public static class OrderExtensions
    {
        public static OrderDto ToDto(this Domain.Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                Store = ToDto(order.Store),
                Items = ToDto(order.Items),
                Customer = ToDto(order.Customer),
                Address = ToDto(order.DeliveryAddress),
                CouponCode = order.Coupon == null ? null : order.Coupon.Id,
                Discount = Math.Round(order.Discount, 2),
                TaxLines = ToDto(order.TaxLines),
                TotalAmount = Math.Round(order.TotalAmount, 2),
            };
        }

        private static StoreDto ToDto(Domain.Store store)
        {
            return new StoreDto
            {
                Id = store.Id,
                AddressLine1 = store.AddressLine1,
                AddressLine2 = store.AddressLine2,
                City = store.City,
                State = store.State,
                PinCode = store.PinCode
            };
        }

        private static List<TaxLineDto> ToDto(List<TaxLineItem> taxLines)
        {
            if (taxLines == null)
            {
                return null;
            }

            List<TaxLineDto> dto = new List<TaxLineDto>();
            foreach (var line in taxLines)
            {
                dto.Add(ToDto(line));
            }

            return dto;
        }

        private static TaxLineDto ToDto(TaxLineItem line)
        {
            if (line == null)
            {
                return null;
            }

            TaxLineDto dto = new TaxLineDto
            {
                Name = line.Name,
                Amount = Math.Round(line.Amount, 2)
            };

            return dto;
        }

        private static AddressDto ToDto(DeliveryAddress deliveryAddress)
        {
            if (deliveryAddress == null)
            {
                return null;
            }

            AddressDto dto = new AddressDto
            {
                AddressLine1 = deliveryAddress.AddressLine1,
                AddressLine2 = deliveryAddress.AddressLine2,
                City = deliveryAddress.City,
                State = deliveryAddress.State,
                PinCode = deliveryAddress.PinCode
            };

            return dto;
        }

        private static CustomerDto ToDto(Customer customer)
        {
            if (customer == null)
            {
                return null;
            }

            CustomerDto dto = new CustomerDto
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Mobile = customer.Mobile
            };

            return dto;
        }

        private static List<LineItemDto> ToDto(List<LineItem> items)
        {
            if (items == null)
            {
                return null;
            }

            List<LineItemDto> dto = new List<LineItemDto>();
            foreach (var item in items)
            {
                dto.Add(item.ToDto());
            }

            return dto;
        }
    }
}
