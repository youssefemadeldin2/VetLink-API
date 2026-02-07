namespace VetLink.Services.Services.AccountService.Dtos.AdminDtos
{
    public class StatisticsDto
	{
		public List<DailyStat> DailyStats { get; set; } = new();
		public List<MonthlyStat> MonthlyStats { get; set; } = new();
		public decimal TotalRevenue { get; set; }
		public int TotalOrders { get; set; }
		public int TotalUsers { get; set; }
	}
}
