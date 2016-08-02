namespace Order.Domain
{
    public class TaxLineItem
    {
        public string Name { get; private set; }
        public double Amount { get; private set; }

        public TaxLineItem(string name, double amount)
        {
            this.Name = name;
            this.Amount = amount;
        }
    }
}