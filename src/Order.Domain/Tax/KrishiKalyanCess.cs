namespace Order.Domain.Tax
{
    public class KrishiKalyanCess : ITaxStep
    {
        public ITaxStep NextStep { get; set; }

        public void Calculate(TaxContext context)
        {
            context.KrishiKalyanCess = 0.5 / 100 * (context.BaseAmount + context.ServiceCharge + context.StateVat);
            context.TaxLines.Add(new TaxLineItem("Krishi Kalyan Cess @ 0.5%", context.KrishiKalyanCess));
            if (this.NextStep != null)
            {
                this.NextStep.Calculate(context);
            }
        }
    }
}
