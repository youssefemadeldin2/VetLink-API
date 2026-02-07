using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class AddressConfigurations : IEntityTypeConfiguration<Address>
	{
		public void Configure(EntityTypeBuilder<Address> builder)
		{
			builder.HasKey(U => U.Id);

			builder.HasOne(a => a.User)
					.WithMany(u => u.Addresses)
					.HasForeignKey(a => a.UserId)
					.OnDelete(DeleteBehavior.Restrict);

			builder.Property(a => a.Country)
					.HasDefaultValue("Egypt");
			builder.Property(a => a.IsDefault)
					.HasDefaultValue(false);

		}
	}
}
