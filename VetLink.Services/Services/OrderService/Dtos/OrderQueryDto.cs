using VetLink.Data.Enums;

namespace VetLink.Services.Services.OrderService.Dtos
{
    public class OrderQueryDto
	{
		public int? BuyerId { get; set; }
		public OredrStatus? Status { get; set; }
	}

}
