namespace VetLink.Services.Services.AccountService.MailService.Dtos
{
    public class OtpEmailModel : EmailModel
	{
		public string OtpCode { get; set; }
		public int OtpExpiryMinutes { get; set; }
	}

}
