using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
	{
		public void Configure(EntityTypeBuilder<OrderItem> builder)
		{
			builder.HasKey(U => U.Id);
			builder.HasOne(oi => oi.Order)
				   .WithMany(o => o.OrderItems)
				   .HasForeignKey(oi => oi.OrderId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.Cascade);
			builder.HasOne(oi => oi.Product)
				   .WithMany(p => p.OrderItems)
				   .HasForeignKey(oi => oi.ProductId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.Restrict);

			builder.HasIndex(oi => oi.OrderId);
			builder.HasIndex(oi => oi.ProductId);
		}
	}
}
