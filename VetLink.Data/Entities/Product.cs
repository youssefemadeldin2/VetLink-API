using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VetLink.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
		[Column(TypeName = "decimal(10,2)")]
		public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string SKU { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public DateOnly ExpireDate { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? DeletedAt { get; set; }
		public Seller Seller { get; set; }
		public Brand Brand { get; set; }
		public Category Category { get; set; }
		public List<OrderItem> OrderItems { get; set; }
		public List<Review> Reviews { get; set; } = new List<Review>();
		[JsonIgnore]
		public List<Image> Images { get; set; } = new List<Image>();
        public ProductState ProductStats { get; set; }
		public ICollection<ReturnItem> ReturnItems { get; set; }
	}
}