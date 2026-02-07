using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class NotificationConfigurations : IEntityTypeConfiguration<Notification>
	{
		public void Configure(EntityTypeBuilder<Notification> builder)
		{
			builder.HasKey(U => U.Id);
			builder.Property(n => n.IsRead).HasDefaultValue(false);

			builder.HasOne(n => n.User)
					.WithMany(u => u.Notifications)
					.HasForeignKey(n => n.UserId)
					.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
