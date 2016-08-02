namespace Order.Domain.Tax
{
    public interface ITaxStep
    {
        void Calculate(TaxContext context);
        ITaxStep NextStep { get; set; }
    }
}
