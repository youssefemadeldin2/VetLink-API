using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class CouponConfigurations : IEntityTypeConfiguration<Coupon>
	{
		public void Configure(EntityTypeBuilder<Coupon> builder)
		{
			builder.HasKey(U => U.Id);
			builder.HasIndex(u => u.Code).IsUnique();
		}
	}
}
