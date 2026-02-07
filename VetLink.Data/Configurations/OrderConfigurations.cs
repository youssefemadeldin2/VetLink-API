using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.HasKey(U => U.Id);

			builder.HasOne(o => o.Buyer)
				   .WithMany(u => u.Orders)
				   .HasForeignKey(o => o.BuyerId);


			//builder.HasOne(o => o.Coupon)
			//		.WithMany(c => c.Orders)
			//		.HasForeignKey(o => o.CouponId)
			//		.OnDelete(DeleteBehavior.Restrict);

			//builder.HasMany(o => o.Returns)
			//		.WithOne(r => r.Order)
			//		.HasForeignKey(r => r.OrderId)
			//		.OnDelete(DeleteBehavior.Restrict);

			//builder.HasMany(o => o.Shipments)
			//			.WithOne(s => s.Order)
			//			.HasForeignKey(s => s.OrderId)
			//			.OnDelete(DeleteBehavior.Cascade);

			builder.HasIndex(o => o.Status);
			builder.HasIndex(o => o.CreatedAt);
			builder.HasIndex(o => o.BuyerId);
			builder.HasIndex(o => new { o.BuyerId, o.Status });
			builder.HasIndex(o => new { o.CreatedAt, o.Status });

		}
	}
}
