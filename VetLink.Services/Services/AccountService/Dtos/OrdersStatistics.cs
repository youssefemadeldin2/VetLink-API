namespace VetLink.Services.Services.AccountService.Dtos
{
    public class OrdersStatistics
	{
		public int Total { get; set; }
		public int Pending { get; set; }
		public int Completed { get; set; }
		public int Cancelled { get; set; }
	}
}
