using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class CategoryConfigurations : IEntityTypeConfiguration<Category>
	{
		public void Configure(EntityTypeBuilder<Category> builder)
		{
			builder.HasKey(U => U.Id);
			builder.HasIndex(u => u.Name).IsUnique();

			builder.HasOne(c => c.ParentCategory)
				   .WithMany(c => c.SubCategories)
				   .HasForeignKey(c => c.ParentId)
				   .OnDelete(DeleteBehavior.NoAction);
		}
	}
}
