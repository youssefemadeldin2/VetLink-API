namespace VetLink.Services.Services.AccountService.MailService.Dtos
{
	public class EmailBrandingOptions
	{
		public string ProductName { get; set; } = "VetLink";
		public string PrimaryColor { get; set; } = "#667eea";
		public string SecondaryColor { get; set; } = "#764ba2";
		public string LogoUrl { get; set; } = "https://vetlink.com/logo.png";
		public string BaseUrl { get; set; } = "https://vetlink.com";
		public bool EnableDarkMode { get; set; } = false;
	}

}
