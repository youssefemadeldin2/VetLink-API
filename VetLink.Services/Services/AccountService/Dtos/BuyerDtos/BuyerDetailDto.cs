namespace VetLink.Services.Services.AccountService.Dtos.BuyerDtos
{
    public class BuyerDetailDto : BuyerDto
	{
		public DateTime? LastLoginAt { get; set; }
		public bool IsActive { get; set; }
		public List<RecentOrderDto> RecentOrders { get; set; } = new();
	}

}
