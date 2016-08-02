namespace Order.Domain.Coupon
{
    public interface ICouponRepository
    {
        ICoupon GetByCode(string couponCode);
    }
}
