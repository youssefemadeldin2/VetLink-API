namespace VetLink.Services.Services.AccountService.Dtos.AuthDtos
{
    public class VerifyEmailDto
	{
		public string Email { get; set; } = string.Empty;
		public string Otp { get; set; } = string.Empty;
		public string Nonce { get; set; } = string.Empty;
	}
}
