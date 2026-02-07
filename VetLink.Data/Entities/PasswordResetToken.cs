using System.ComponentModel.DataAnnotations;

namespace VetLink.Data.Entities
{
    public class PasswordResetToken
	{
		[Key]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string Token { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;

	}
}