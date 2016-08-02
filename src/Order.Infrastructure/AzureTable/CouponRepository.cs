using System;
using Order.Domain.Coupon;

namespace Order.Infrastructure.AzureTable
{
    public class CouponRepository : ICouponRepository
    {
        public ICoupon GetByCode(string couponCode)
        {
            // TODO: Hook storage.
            return new XPercentOnPurchaseAboveN(couponCode);
        }
    }
}
