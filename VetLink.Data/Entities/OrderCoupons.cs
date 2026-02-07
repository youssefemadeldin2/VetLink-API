using System.ComponentModel.DataAnnotations.Schema;

namespace VetLink.Data.Entities
{
    public class OrderCoupons
	{
		public int OrderId { get; set; }
		public int CouponId { get; set; }
		[Column(TypeName = "decimal(10,2)")]
		public decimal DiscountAmount { get; set; }
		public string CouponCode { get; set; }
		public Order Order { get; set; }
        public Coupon Coupon { get; set; }
    }
}