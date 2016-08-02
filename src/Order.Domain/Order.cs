using System;
using System.Collections.Generic;
using System.Linq;
using Order.Domain.Coupon;
using Order.Domain.Tax;

namespace Order.Domain
{
    public class Order
    {
        public OrderState State { get; private set; }
        public ITaxStep RootTaxStep { get; private set; }
        public string Id { get; private set; }

        public Store Store { get; private set; }
        public List<LineItem> Items { get; private set; }
        public Customer Customer { get; private set; }
        public DeliveryAddress DeliveryAddress { get; private set; }
        public PaymentDetails PaymentDetails { get; private set; }
        public ICoupon Coupon { get; private set; }
        public double Discount { get; private set; }
        public List<TaxLineItem> TaxLines { get; set; }
        public double TotalAmount { get; private set; }

        // Introduced assist serialization from repository. Otherwise, we'd have to create a 'snapshot' dto.
        private Order()
        {
        }

        public Order(string id, Store store, ITaxStep rootTaxStep)
        {
            if (string.IsNullOrWhiteSpace(store.PinCode))
            {
                throw new ArgumentException("Store must have a pincode.");
            }

            this.Id = id;
            this.Store = store;
            this.RootTaxStep = rootTaxStep;
            this.State = OrderState.Created;
            this.Items = new List<LineItem>();
            this.TaxLines = new List<TaxLineItem>();
        }

        public void AddLineItem(LineItem item)
        {
            if (this.State != OrderState.Created)
            {
                throw new InvalidOperationException("Invalid state.");
            }

            var existingItem = this.Items.FirstOrDefault(tuple => tuple.ProductEquals(item));
            if (existingItem != null)
            {
                existingItem.Merge(item);
            }
            else
            {
                this.Items.Add(item);
            }

            this.UpdateTotal();
        }

        public void RemoveLineItem(string itemId)
        {
            if (this.State != OrderState.Created)
            {
                throw new InvalidOperationException("Invalid state.");
            }

            var item = this.Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                this.Items.Remove(item);
            }

            this.UpdateTotal();
        }

        public void Checkout()
        {
            if (this.State != OrderState.Created)
            {
                throw new InvalidOperationException("Invalid state.");
            }

            if (this.Items.Count == 0)
            {
                throw new InvalidOperationException("No item to checkout.");
            }

            this.State = OrderState.Checkedout;
            this.UpdateTotal();
        }

        public void ApplyCoupon(ICoupon coupon)
        {
            if (this.State != OrderState.Checkedout)
            {
                throw new InvalidOperationException("Invalid state.");
            }

            if (this.Coupon != null)
            {
                throw new InvalidOperationException("A coupon is already applied to this order.");
            }

            if (!coupon.IsApplicable(this))
            {
                throw new InvalidOperationException("Invalid coupon");
            }

            this.Coupon = coupon;
            this.UpdateTotal();
        }

        public void SetCustomerDetails(string firstName, string lastName, string email, string mobile)
        {
            if (this.State != OrderState.Checkedout)
            {
                throw new InvalidOperationException("Invalid state.");
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("Invalid firstName.");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Invalid lastName.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                // TODO: Some regex match.
                throw new ArgumentException("Invalid email.");
            }

            if (string.IsNullOrWhiteSpace(mobile))
            {
                // TODO: Some regex match.
                throw new ArgumentException("Invalid mobile.");
            }

            this.Customer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Mobile = mobile
            };
        }

        public void SetDeliveryAddress(string addressLine1, string addressLine2, string city, string state, string pinCode)
        {
            if (this.State != OrderState.Checkedout)
            {
                throw new InvalidOperationException("Invalid state.");
            }

            if (string.IsNullOrWhiteSpace(addressLine1))
            {
                throw new ArgumentException("Invalid addressLine1.");
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException("Invalid city.");
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                throw new ArgumentException("Invalid state.");
            }

            if (string.IsNullOrWhiteSpace(pinCode))
            {
                throw new ArgumentException("Invalid pinCode.");
            }

            this.DeliveryAddress = new DeliveryAddress
            {
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                City = city,
                State = state,
                PinCode = pinCode
            };

            this.UpdateTotal();
        }

        public void PaymentComplete(string bankName, string transactionId)
        {
            if (this.State != OrderState.Checkedout)
            {
                throw new InvalidOperationException("Invalid state.");
            }

            this.PaymentDetails = new PaymentDetails
            {
                BankName = bankName,
                TransactionId = transactionId
            };

            this.State = OrderState.PaymentComplete;
        }

        public void SetDiscount(double value)
        {
            this.Discount = value;
        }

        private void UpdateTotal()
        {
            foreach (var item in this.Items)
            {
                item.CalculateAmount();
            }

            this.TotalAmount = this.Items.Sum(i => i.Amount);

            // Apply coupon.
            if (this.Coupon != null)
            {
                this.Coupon.Apply(this);
            }

            // Recalculate after applying coupon.
            this.TotalAmount = this.Items.Sum(i => i.Amount) - this.Discount;

            // Calculate taxes.
            TaxContext context = new TaxContext
            {
                BaseAmount = this.TotalAmount,
                PinCode = this.Store.PinCode,
                TaxLines = new List<TaxLineItem>()
            };
            this.RootTaxStep.Calculate(context);
            this.TaxLines = context.TaxLines;

            // Update Total.
            this.TotalAmount += this.TaxLines.Sum(t => t.Amount);
        }
    }
}
