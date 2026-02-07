namespace VetLink.Services.Services.AccountService.Dtos.SellerDtos
{
    public class SellerDashboardDto
	{
		public int TotalProducts { get; set; }
		public int TotalOrders { get; set; }
		public decimal TotalRevenue { get; set; }
		public int PendingOrders { get; set; }
		public List<RecentOrderDto> RecentOrders { get; set; } = new();
	}
}
