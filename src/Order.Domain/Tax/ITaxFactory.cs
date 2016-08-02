using System.Collections.Generic;

namespace Order.Domain.Tax
{
    public interface ITaxFactory
    {
        ITaxStep GetRootStep();
    }
}
