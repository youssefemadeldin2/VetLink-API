using System.ComponentModel.DataAnnotations.Schema;

namespace VetLink.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
		[ForeignKey("ParentCategory")]
		public int? ParentId { get; set; }
        public string Name { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    }
}