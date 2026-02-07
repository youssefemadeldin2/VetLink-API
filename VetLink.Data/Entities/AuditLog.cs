using System.ComponentModel.DataAnnotations;
using VetLink.Data.Enums;

namespace VetLink.Data.Entities
{
    public class AuditLog
	{
		[Key]
		public int Id { get; set; }
		public int? UserId { get; set; }
		public string UserEmail { get; set; }
		public AuditAction Action { get; set; }
		public string Details { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		//navigation property
		public User User { get; set; }
	}
}