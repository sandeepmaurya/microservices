using System.Collections.Generic;

namespace Order.Domain.Tax
{
    public class TaxContext
    {
        public double BaseAmount { get; set; }
        public string PinCode { get; set; }
        public double ServiceCharge { get; set; }
        public double ServiceTax { get; set; }
        public double EducationCess { get; set; }
        public double StateVat { get; set; }
        public double SwachhBharatCess { get; set; }
        public double KrishiKalyanCess { get; set; }
        public List<TaxLineItem> TaxLines { get; set; }
    }
}
