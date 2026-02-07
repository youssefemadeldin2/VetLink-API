using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class OrderCouponsConfigurations : IEntityTypeConfiguration<OrderCoupons>
	{
		public void Configure(EntityTypeBuilder<OrderCoupons> builder)
		{
			builder.HasKey(oc => new { oc.OrderId, oc.CouponId });
		}
	}
}
