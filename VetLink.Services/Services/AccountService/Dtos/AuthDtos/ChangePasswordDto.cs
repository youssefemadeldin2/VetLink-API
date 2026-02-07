namespace VetLink.Services.Services.AccountService.Dtos.AuthDtos
{
    public class ChangePasswordDto
	{
		public string CurrentPassword { get; set; } = string.Empty;
		public string NewPassword { get; set; } = string.Empty;
	}
}
