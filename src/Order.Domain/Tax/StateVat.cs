namespace Order.Domain.Tax
{
    public class StateVat : ITaxStep
    {
        public ITaxStep NextStep { get; set; }

        public void Calculate(TaxContext context)
        {
            if (context.PinCode != null)
            {
                string state = context.PinCode.Substring(0, 2);
                double percent = 0;
                switch (state)
                {
                    case "50":
                        percent = 12.6 / 100;
                        break;
                    default:
                        percent = 14.0 / 100;
                        break;
                }

                context.StateVat = percent * (context.BaseAmount + context.ServiceCharge);
                context.TaxLines.Add(new TaxLineItem("VAT @ " + percent * 100 + "%", context.StateVat));
            }

            if (this.NextStep != null)
            {
                this.NextStep.Calculate(context);
            }
        }
    }
}
