namespace VetLink.Services.Services.AccountService.Dtos.AuthDtos
{
    public class RecentSignupDto
	{
		public int Id { get; set; }
		public string Email { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
	}
}
