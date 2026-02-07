using System.ComponentModel.DataAnnotations;

namespace VetLink.Services.Services.ReviewService.Dtos
{
    public class UpdateReviewDto
	{
		[Required]
		public int Id { get; set; }

		[Required]
		public int BuyerId { get; set; }

		[Range(1, 5)]
		public int Rating { get; set; }

		[StringLength(500)]
		public string? Comment { get; set; }

		public bool ShowName { get; set; }
	}
}
