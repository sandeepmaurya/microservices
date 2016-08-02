namespace Order.Domain.Coupon
{
    public class XPercentOnPurchaseAboveN : CouponBase
    {
        public XPercentOnPurchaseAboveN(string id) : base(id)
        {
        }

        public override void Apply(Order order)
        {
            if (order.TotalAmount > 500.0)
            {
                order.SetDiscount(10.0 / 100 * order.TotalAmount);
            }
        }

        public override bool IsApplicable(Order order)
        {
            if (order.TotalAmount > 500.0)
            {
                return true;
            }

            return false;
        }
    }
}
