using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VetLink.Data.Enums;

namespace VetLink.Data.Entities
{
    public class Coupon
	{
        public int Id { get; set; }
		[MaxLength(50)]
		public string Code { get; set; }
		public CouponScope Scope { get; set; }
		public CouponType Type { get; set; }
		[Column(TypeName = "decimal(10,2)")]
		public decimal Value { get; set; }
		public decimal? MinOrderAmount { get; set; }
		public int MaxUsage { get; set; }
        public int UsageCount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int CreatorId { get; set; }
		public int? ProductId { get; set; }
		public Product? Product { get; set; }
		public User Creator { get; set; }
        public List<OrderCoupons> OrderCoupons { get; set; } = new List<OrderCoupons>();


	}
}