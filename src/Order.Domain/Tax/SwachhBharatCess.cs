namespace Order.Domain.Tax
{
    public class SwachhBharatCess : ITaxStep
    {
        public ITaxStep NextStep { get; set; }

        public void Calculate(TaxContext context)
        {
            context.SwachhBharatCess = 0.5 / 100 * (context.BaseAmount + context.ServiceCharge + context.StateVat);
            context.TaxLines.Add(new TaxLineItem("Swachh Bharat Cess @ 0.5%", context.SwachhBharatCess));
            if (this.NextStep != null)
            {
                this.NextStep.Calculate(context);
            }
        }
    }
}
