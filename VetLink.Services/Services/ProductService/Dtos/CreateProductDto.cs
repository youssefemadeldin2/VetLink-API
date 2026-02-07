using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VetLink.Services.Services.ProductService.Dtos
{
    public class CreateProductDto
    {
        [Required]
        public int SellerId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
        [Required]
        public DateOnly ExpireDate { get; set; }

        public List<IFormFile>? Images { get; set; }
		public int PrimaryImageIndex { get; set; } = 0;
	}
}
