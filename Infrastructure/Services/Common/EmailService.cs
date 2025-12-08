using System.Net;
using System.Net.Mail;
using Core.Interfaces.Common;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Common
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body, byte[]? attachment = null, string? attachmentName = null)
        {
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
            var smtpUser = _configuration["Smtp:User"];
            var smtpPass = _configuration["Smtp:Password"];
            var smtpSsl = bool.Parse(_configuration["Smtp:EnableSsl"] ?? "true");
            var smtpFrom = _configuration["Smtp:From"] ?? smtpUser;

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
            {
                throw new InvalidOperationException("SMTP configuration is missing in appsettings.json");
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = smtpSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpFrom!),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            if (attachment != null && !string.IsNullOrEmpty(attachmentName))
            {
                mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment), attachmentName));
            }

            await client.SendMailAsync(mailMessage);
        }
    }
}
