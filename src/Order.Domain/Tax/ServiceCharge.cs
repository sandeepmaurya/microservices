namespace Order.Domain.Tax
{
    public class ServiceCharge : ITaxStep
    {
        public ITaxStep NextStep { get; set; }

        public void Calculate(TaxContext context)
        {
            context.ServiceCharge = 10.0 / 100 * context.BaseAmount;
            context.TaxLines.Add(new TaxLineItem("Service Charge @ 10%", context.ServiceCharge));
            if (this.NextStep != null)
            {
                this.NextStep.Calculate(context);
            }
        }
    }
}
