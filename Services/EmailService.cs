using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using HonestAuto.Models;
using HonestAuto.Data;

// Source 1: https://www.youtube.com/watch?v=lCHKwyekbT4
// Source 2: https://mailtrap.io/blog/asp-net-core-send-email/
// Source 3: https://stackoverflow.com/questions/8628683/how-to-send-html-formatted-email
// Source 4: https://net-informations.com/csharp/communications/csharp-html-email.htm
namespace HonestAuto.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly MarketplaceContext _context;

        // Constructor injecting email settings and a database context
        public EmailService(IOptions<EmailSettings> emailSettings, MarketplaceContext context)
        {
            _emailSettings = emailSettings.Value;
            _context = context;
        }

        // Method to send email asynchronously
        public async Task SendEmailAsync(string email, string subject, string message, bool isHtml = false)
        {
            // Create a new MimeMessage instance
            var emailMessage = new MimeMessage();

            // Set the sender of the email
            emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Email));

            // Set the recipient of the email
            emailMessage.To.Add(new MailboxAddress("", email));

            // Set the subject of the email
            emailMessage.Subject = subject;

            // Set the body of the email based on whether it's HTML or plain text
            if (isHtml)
            {
                emailMessage.Body = new TextPart("html") { Text = message };
            }
            else
            {
                emailMessage.Body = new TextPart("plain") { Text = message };
            }

            // Create a new SmtpClient instance
            using (var client = new SmtpClient())
            {
                // Connect to the SMTP server
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

                // Authenticate with the SMTP server using provided credentials
                await client.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);

                // Send the email message
                await client.SendAsync(emailMessage);

                // Disconnect from the SMTP server
                await client.DisconnectAsync(true);
            }
        }
    }
}