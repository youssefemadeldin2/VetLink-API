using VetLink.Services.Services.CategoryService.Dtos;
using VetLink.Services.Services.ReviewService.Dtos;

namespace VetLink.Services.Services.ProductService.Dtos
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        //public decimal? CompareAtPrice { get; set; }
        public int StockQuantity { get; set; }
        //public int LowStockThreshold { get; set; }
        public string SKU { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        //public bool IsReturnable { get; set; }
        //public int ReturnPeriodDays { get; set; }
        public DateTime ExpiryDate { get; set; }
        //public int ExpiryAlertDays { get; set; }

        public ProductSeller Seller { get; set; } = null!;
        public CategoryDto Category { get; set; } = null!;
        public BrandDto Brand { get; set; } = null!;
        public List<ImageDto> Images { get; set; } = new();

        //public ProductStatsDto Stats { get; set; } = null!;
        public List<ShowReviewDto> Reviews { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
