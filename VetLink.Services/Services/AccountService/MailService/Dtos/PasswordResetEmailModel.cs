namespace VetLink.Services.Services.AccountService.MailService.Dtos
{
    public class PasswordResetEmailModel : EmailModel
	{
		public string ResetLink { get; set; }
		public int LinkExpiryHours { get; set; }
	}

}
