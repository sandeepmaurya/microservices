namespace Order.Domain.Coupon
{
    public interface ICoupon
    {
        string Id { get; }
        bool IsApplicable(Order order);
        void Apply(Order order);
    }
}
