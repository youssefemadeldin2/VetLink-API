using System.ComponentModel.DataAnnotations;

namespace VetLink.Data.Entities
{
    public class Review
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
		public byte Rating { get; set; }

		public string? Comment { get; set; }

		public bool IsApproved { get; set; } = true;
		public bool ShowName { get; set; } = true;

		public DateTime CreatedAt { get; set; } = DateTime.Now;

		// Navigational Properties
		public int ProductId { get; set; }
		public Product Product { get; set; }

		public int BuyerId { get; set; }
		public User Buyer { get; set; }
	}
}