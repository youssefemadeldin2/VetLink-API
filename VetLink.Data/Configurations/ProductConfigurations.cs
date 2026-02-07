using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.HasKey(U => U.Id);
			builder.HasIndex(u => u.SKU).IsUnique();
			builder.HasIndex(u => u.Name);
			builder.HasIndex(u => u.BrandId);
			builder.HasIndex(u => u.Price);
			builder.HasIndex(u => u.CategoryId);
			builder.HasIndex(u => u.SellerId);
			builder.HasIndex(p => new { p.SellerId, p.IsActive });
			builder.HasIndex(p => new { p.Name, p.IsActive });

			builder.HasOne(p => p.Category)
					.WithMany(c => c.Products)
					.HasForeignKey(p => p.CategoryId)
					.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(p => p.Brand)
					.WithMany(b => b.Products)
					.HasForeignKey(p => p.BrandId)
					.IsRequired();

		}
	}
}
