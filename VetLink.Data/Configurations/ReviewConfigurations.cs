using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class ReviewConfigurations : IEntityTypeConfiguration<Review>
	{
		public void Configure(EntityTypeBuilder<Review> builder)
		{
			builder.HasKey(U => U.Id);

			builder.HasOne(r => r.Buyer)
					.WithMany(u => u.Reviews)
					.HasForeignKey(r => r.BuyerId)
					.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(r => r.Product)
					.WithMany(p => p.Reviews)
					.HasForeignKey(r => r.ProductId)
					.OnDelete(DeleteBehavior.Cascade);

			builder.HasIndex(r => r.ProductId);
			builder.HasIndex(r => r.BuyerId);
			builder.HasIndex(r => new { r.ProductId, r.IsApproved });
			builder.HasIndex(r => new { r.BuyerId, r.ProductId }).IsUnique();
			builder.Property(a => a.IsApproved)
					.HasDefaultValue(true);
		}
	}
}
