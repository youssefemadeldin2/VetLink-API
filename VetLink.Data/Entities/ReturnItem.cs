using Microsoft.EntityFrameworkCore;

namespace VetLink.Data.Entities
{
    [PrimaryKey("ReturnId", "ProductId")]
	public class ReturnItem
	{
		public int ReturnId { get; set; }   // Composite FK
		public int ProductId { get; set; }  // Composite FK
		public int Quantity { get; set; }

		public virtual Return Return { get; set; }
		public virtual Product Product { get; set; }
	}
}