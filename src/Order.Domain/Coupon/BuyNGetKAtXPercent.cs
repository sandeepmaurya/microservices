using System;

namespace Order.Domain.Coupon
{
    public class BuyNGetKAtXPercent : CouponBase
    {
        public BuyNGetKAtXPercent(string id) : base(id)
        {
        }

        public override void Apply(Order order)
        {
            throw new NotImplementedException();
        }

        public override bool IsApplicable(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
