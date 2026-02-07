namespace VetLink.Services.Services.AccountService.MailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlMessage);
    }
}
