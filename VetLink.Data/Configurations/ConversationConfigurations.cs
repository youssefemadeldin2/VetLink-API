using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VetLink.Data.Entities;

namespace VetLink.Data.Configurations
{
    public class ConversationConfigurations : IEntityTypeConfiguration<Conversation>
	{
		public void Configure(EntityTypeBuilder<Conversation> builder)
		{
			builder.HasOne(e => e.ParticipantOne)
				  .WithMany(u => u.ConversationsAsParticipant1)
				  .HasForeignKey(e => e.ParticipantOneId)
				  .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete issues

			// Configure foreign key for Participant2
			builder.HasOne(e => e.ParticipantTwo)
				  .WithMany(u => u.ConversationsAsParticipant2)
				  .HasForeignKey(e => e.ParticipantTwoId)
				  .OnDelete(DeleteBehavior.Restrict);
		}
	}
}
