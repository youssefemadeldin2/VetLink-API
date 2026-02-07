using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class WishlistConfigurations : IEntityTypeConfiguration<Wishlist>
	{
		public void Configure(EntityTypeBuilder<Wishlist> builder)
		{
			builder.HasKey(wp => new { wp.UserId, wp.ProductId });
		}
	}
}
