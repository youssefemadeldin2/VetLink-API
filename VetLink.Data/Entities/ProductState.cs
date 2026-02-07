using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetLink.Data.Entities
{
    public class ProductState
	{
		[Key]
		[ForeignKey("User")]
		public int ProductId { get; set; }
        public int ViewCount { get; set; }=0;
		[Column(TypeName = "decimal(3,2)")]
		public decimal AverageRate { get; set; } = 0;
        public Product Product { get; set; }
    }
}