namespace VetLink.Services.Services.AccountService.MailService.Dtos
{
    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string SenderEmail { get; set; }
        public string AppPassword { get; set; }
    }

}
