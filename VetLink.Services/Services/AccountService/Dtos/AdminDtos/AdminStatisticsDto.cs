namespace VetLink.Services.Services.AccountService.Dtos.AdminDtos
{
    public class AdminStatisticsDto
	{
		public UsersStatistics Users { get; set; } = new();
		public OrdersStatistics Orders { get; set; } = new();
		public RevenueStatistics Revenue { get; set; } = new();
	}
}
