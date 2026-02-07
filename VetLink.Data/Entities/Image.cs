using System.ComponentModel.DataAnnotations;

namespace VetLink.Data.Entities
{
    public class Image
	{
		[Key]
		public int Id { get; set; }
		public int ProductId { get; set; }
		[Required]
		public string ImageURL { get; set; } = null!;
		[Required]
		public string PublicId { get; set; } = null!; 
		public bool IsPrimary { get; set; } = false;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public Product Product { get; set; } = null!;
	}
}