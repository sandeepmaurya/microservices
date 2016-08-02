using Order.Domain.Tax;

namespace Order.Infrastructure.Factories
{
    public class TaxFactoryNoServiceCharge : ITaxFactory
    {
        public ITaxStep GetRootStep()
        {
            return new ServiceTax
            {
                NextStep = new StateVat
                {
                    NextStep = new EducationCess
                    {
                        NextStep = new SwachhBharatCess
                        {
                            NextStep = new KrishiKalyanCess()
                        }
                    }
                }
            };
        }
    }
}
