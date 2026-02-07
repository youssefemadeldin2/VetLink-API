using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace VetLink.Services.Services.ProductService.Dtos
{
    public class UpdateProductDto
    {
        [Required]
        [NotNull]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsFeatured { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
    }

}
