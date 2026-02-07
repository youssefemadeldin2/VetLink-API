using System.ComponentModel.DataAnnotations.Schema;

namespace VetLink.Services.Services.OrderService.Dtos
{
    public class OrderItemDto
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public string ProductName { get; set; }
		public decimal TotalPrice { get; set; }
	}
}
