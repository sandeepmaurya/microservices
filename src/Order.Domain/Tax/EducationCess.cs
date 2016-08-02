namespace Order.Domain.Tax
{
    public class EducationCess : ITaxStep
    {
        public ITaxStep NextStep { get; set; }

        public void Calculate(TaxContext context)
        {
            context.EducationCess = 3.0 / 100 * context.ServiceTax;
            context.TaxLines.Add(new TaxLineItem("Education Cess @ 3%", context.EducationCess));
            if (this.NextStep != null)
            {
                this.NextStep.Calculate(context);
            }
        }
    }
}
