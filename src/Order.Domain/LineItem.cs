using System;

namespace Order.Domain
{
    public class LineItem
    {
        private LineItem()
        {
        }

        public LineItem(string id, Product product, int quantity)
        {
            this.Id = id;
            this.Product = product;
            this.Quantity = quantity;
        }

        public string Id { get; private set; }
        public Product Product { get; private set; }
        public int Quantity { get; private set; }
        public double Discount { get; private set; }
        public double Amount { get; private set; }

        public void CalculateAmount()
        {
            this.Amount = this.Product.UnitPrice * this.Quantity;
        }

        public void Merge(LineItem item)
        {
            this.Quantity += item.Quantity;
        }

        public bool ProductEquals(LineItem item)
        {
            if (this.Product.Id.Equals(item.Product.Id, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}