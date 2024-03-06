using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using HonestAuto.Models;
using HonestAuto.Data;

namespace HonestAuto.Services

{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly MarketplaceContext _context;

        public EmailService(IOptions<EmailSettings> emailSettings, MarketplaceContext context)
        {
            _emailSettings = emailSettings.Value;
            _context = context;
        }

        public async Task SendEmailAsync(string email, string subject, string message, bool isHtml = false)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Email));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            if (isHtml)
            {
                emailMessage.Body = new TextPart("html") { Text = message };
            }
            else
            {
                emailMessage.Body = new TextPart("plain") { Text = message };
            }

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}