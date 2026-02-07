namespace VetLink.Services.Services.AccountService.Dtos.AdminDtos
{
    public class DailyStat
	{
		public DateTime Date { get; set; }
		public int Orders { get; set; }
		public decimal Revenue { get; set; }
		public int NewUsers { get; set; }
	}
}
