using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class ShipmentConfigurations : IEntityTypeConfiguration<Shipment>
	{
		public void Configure(EntityTypeBuilder<Shipment> builder)
		{
			builder.HasKey(U => U.Id);
			builder.HasIndex(u => u.OrderId);
			builder.HasIndex(u => u.SellerId);
		}
	}
}
