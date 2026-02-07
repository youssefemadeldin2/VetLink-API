using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetLink.Data.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
		[Column(TypeName = "decimal(10,2)")]
		public decimal Price { get; set; }
		[Column(TypeName = "decimal(10,2)")]
		public decimal TotalPrice { get; set; }
		[Timestamp]
		public byte[] RowVersion { get; set; } = null!;
		public Order Order { get; set; } = null!;
		public Product Product { get; set; }
    }
}