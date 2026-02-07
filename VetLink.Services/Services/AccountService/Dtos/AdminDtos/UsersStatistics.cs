namespace VetLink.Services.Services.AccountService.Dtos.AdminDtos
{
    public class UsersStatistics
	{
		public int Total { get; set; }
		public int ActiveToday { get; set; }
		public int NewThisWeek { get; set; }
		public int NewThisMonth { get; set; }
	}
}
