namespace Order.Domain.Coupon
{
    public abstract class CouponBase : ICoupon
    {
        public string Id { get; protected set; }

        public CouponBase(string id)
        {
            this.Id = id;
        }

        public abstract void Apply(Order order);

        public abstract bool IsApplicable(Order order);
    }
}
