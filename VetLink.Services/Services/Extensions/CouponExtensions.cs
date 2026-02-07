using VetLink.Data.Entities;
using VetLink.Data.Enums;

namespace VetLink.Services.Services.Extensions
{
    public static class CouponExtensions
    {
        public static decimal CalculateDiscount(this Coupon coupon, decimal totalAmount)
        {
            if (coupon == null)
                return 0;

            if (totalAmount <= 0)
                return 0;

            return coupon.Type switch
            {
                CouponType.percentage => Math.Round(totalAmount * (coupon.Value / 100m), 2),
                CouponType.Fixed => Math.Min(coupon.Value, totalAmount),
                _ => 0
            };
        }
    }
}
