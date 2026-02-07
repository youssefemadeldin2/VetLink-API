namespace VetLink.Services.Services.AccountService.TokenService.Dtos
{
    public class TokenValidationResultDto
	{
		public bool IsValid { get; set; }
		public DateTime? ExpiresAt { get; set; }
	}
}
