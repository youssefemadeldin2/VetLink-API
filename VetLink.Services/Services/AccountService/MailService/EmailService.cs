using System;
using System.Collections.Generic;
using System.Text;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using VetLink.Services.Services.AccountService.MailService.Dtos;

namespace VetLink.Services.Services.AccountService.MailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
			if (string.IsNullOrWhiteSpace(to))
				throw new ArgumentNullException(nameof(to));

			if (string.IsNullOrWhiteSpace(subject))
				throw new ArgumentNullException(nameof(subject));

			if (string.IsNullOrWhiteSpace(htmlMessage))
				throw new ArgumentNullException(nameof(htmlMessage));

			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse(_settings.SenderEmail));
			email.To.Add(MailboxAddress.Parse(to));
			email.Subject = subject;

			var builder = new BodyBuilder
			{
				HtmlBody = htmlMessage
			};

			email.Body = builder.ToMessageBody();

			using var smtp = new SmtpClient();
			await smtp.ConnectAsync(
				_settings.Host,
				_settings.Port,
				MailKit.Security.SecureSocketOptions.StartTls
			);

			await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.AppPassword);
			await smtp.SendAsync(email);
			await smtp.DisconnectAsync(true);
		}
    }
}