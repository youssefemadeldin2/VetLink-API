namespace VetLink.Services.Services.OrderService.Dtos
{
	public class AddOrderItemDto
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public string? CouponCode { get; set; }
	}
}
