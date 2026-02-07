using System.ComponentModel.DataAnnotations;

namespace VetLink.Data.Entities
{
    public class Notification
	{
		[Key]
		public int Id { get; set; }
		public int? UserId { get; set; }
		public string Message { get; set; }
		public bool IsRead { get; set; } = false;
		public string? Link { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		//navigation property
		public User User { get; set; }
	}
}