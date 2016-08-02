namespace Order.Domain.Tax
{
    public class ServiceTax : ITaxStep
    {
        public ITaxStep NextStep { get; set; }

        public void Calculate(TaxContext context)
        {
            context.ServiceTax = 5.6 / 100 * (context.BaseAmount + context.ServiceCharge + context.StateVat);
            context.TaxLines.Add(new TaxLineItem("Service Tax @ 5.6%", context.ServiceTax));
            if (this.NextStep != null)
            {
                this.NextStep.Calculate(context);
            }
        }
    }
}
