using System.ComponentModel.DataAnnotations;
using VetLink.Data.Enums;

namespace VetLink.Data.Entities
{
    public class SupportTicket
	{
		[Key]
		public int Id { get; set; }
		public int UserId { get; set; }
		public string subject { get; set; }
		public SupportTicketStatus Status { get; set; } = SupportTicketStatus.open;
		public SupportTicketpPiority Piority { get; set; } = SupportTicketpPiority.medium;
		public DateTime CreatedAt { get; set; } = DateTime.Now;


		//nvigation property
		public User User { get; set; }
		public ICollection<TicketReplay> Replays { get; set; } = new List<TicketReplay>();


	}
}