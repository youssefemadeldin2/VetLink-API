using VetLink.Data.Entities;

namespace VetLink.Services.Services.Extensions
{
    public static class CouponValidationExtensions
    {
        public static bool IsValid(this Coupon coupon)
        {
            var now = DateTime.UtcNow;

            if (coupon.UsageCount >= coupon.MaxUsage)
                return false;

            if (now < coupon.ValidFrom || now > coupon.ValidTo)
                return false;

            return true;
        }
    }
}
