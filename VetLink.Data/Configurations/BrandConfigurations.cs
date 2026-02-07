using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class BrandConfigurations : IEntityTypeConfiguration<Brand>
	{
		public void Configure(EntityTypeBuilder<Brand> builder)
		{
			builder.HasKey(U => U.Id);
			builder.HasIndex(u => u.Name).IsUnique();
		}
	}
}
