using VetLink.Data.Enums;

namespace VetLink.Services.Services.OrderService.Dtos
{
    public class CreateOrderDto
	{
		public int? BuyerId { get; set; }
		public int AddressId { get; set; }
		public PaymentMethod PaymentMethod { get; set; }

		public List<AddOrderItemDto> Items { get; set; } = new();
		public List<ApplyCouponDto>? Coupons { get; set; }
	}

}
