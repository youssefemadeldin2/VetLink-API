using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class ReturnConfigurations : IEntityTypeConfiguration<Return>
	{
		public void Configure(EntityTypeBuilder<Return> builder)
		{
			builder.HasKey(U => U.Id);
			builder.HasIndex(r => r.OrderId);
			builder.HasIndex(r => r.BuyerId);
		}
	}
}
