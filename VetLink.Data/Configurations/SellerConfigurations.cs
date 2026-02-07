using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class SellerConfigurations : IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> builder)
        {
            builder.HasKey(u => u.UserId);
			builder.HasOne(s => s.User)
					.WithOne(u => u.Seller)
					.HasForeignKey<Seller>(s => s.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			builder.HasIndex(s => s.StoreName)
			.IsUnique();
		}
    }
}
