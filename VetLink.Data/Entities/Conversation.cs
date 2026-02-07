using System.ComponentModel.DataAnnotations;

namespace VetLink.Data.Entities
{
    public class Conversation
	{
		[Key]
		public int Id { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public int ParticipantOneId { get; set; }
		public int ParticipantTwoId { get; set; }
		public User ParticipantOne { get; set; }
		public User ParticipantTwo { get; set; }
		public ICollection<Message> Messages { get; set; } = new List<Message>();
		public virtual ICollection<User> Users { get; set; } = new List<User>();
	}
}