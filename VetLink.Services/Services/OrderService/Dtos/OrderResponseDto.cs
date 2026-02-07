using VetLink.Data.Enums;

namespace VetLink.Services.Services.OrderService.Dtos
{
    public class OrderResponseDto
	{
		public int Id { get; set; }
		public OredrStatus Status { get; set; }
		public decimal TotalAmount { get; set; }
		public PaymentMethod PaymentMethod { get; set; }
		public PaymentStatus PaymentStatus { get; set; }
		public DateTime CreatedAt { get; set; }

		public List<OrderItemDto> Items { get; set; } = new();
		public List<OrderCouponDto> Coupons { get; set; } = new();
	}
}
