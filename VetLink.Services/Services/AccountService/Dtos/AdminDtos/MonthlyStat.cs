namespace VetLink.Services.Services.AccountService.Dtos.AdminDtos
{
    public class MonthlyStat
	{
		public string Month { get; set; }
		public int Orders { get; set; }
		public decimal Revenue { get; set; }
		public int NewUsers { get; set; }
	}
}
