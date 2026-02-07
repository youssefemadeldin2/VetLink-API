namespace VetLink.Services.Services.OrderService.Dtos
{
    public class OrderItemSummaryDto
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }

		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal TotalPrice => Quantity * UnitPrice;
	}
}
