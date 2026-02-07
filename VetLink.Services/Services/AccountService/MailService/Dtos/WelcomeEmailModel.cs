namespace VetLink.Services.Services.AccountService.MailService.Dtos
{
    public class WelcomeEmailModel : EmailModel
	{
		public string DashboardUrl { get; set; }
		public string SupportEmail { get; set; }
	}

}
