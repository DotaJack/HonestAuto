using HonestAuto.Models;
using HonestAuto.Services;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HonestAuto.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ChatMessageService _messageService;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;

        public ChatHub(ChatMessageService messageService, UserManager<User> userManager, EmailService emailService)
        {
            _messageService = messageService;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task SendMessage(string receiverId, string messageContent)
        {
            var senderId = Context.UserIdentifier;
            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                DateSent = DateTime.UtcNow,
                Content = messageContent
            };

            await _messageService.SaveMessageAsync(message);

            var receiver = await _userManager.FindByIdAsync(receiverId);
            var receiverEmail = receiver != null ? await _userManager.GetEmailAsync(receiver) : null;

            if (receiverEmail != null)
            {
                var sender = await _userManager.FindByIdAsync(senderId);
                var senderEmail = sender?.Email ?? "a user"; // Use a default value if sender's email is null

                var emailSubject = "New Chat Message";

                var emailContent = $@"
<html>
<head>
    <title>New Chat Message</title>
</head>
<body style=""background-color: #f2f2f2; font-family: Arial, sans-serif;"">
    <table cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px; margin: auto; background-color: #ffffff;"">
        <tr>
            <td style=""padding: 20px; text-align: left;"">

      <h1>Hi,</h1>
            <p>You have received a new message from <strong>{senderEmail ?? "a user"}</strong>:</p>
            <blockquote style='font-style: italic; margin: 20px 0;'>{messageContent}</blockquote>
            <p>Click <a href='https://localhost:7016/Chat' style='color: #ADD8E6; font-weight: bold;'>here</a> to view the message.</p>
            <p>Thanks,<br>The Honest Auto Team</p>
            </td>
        </tr>
        <tr>
            <td style=""padding: 20px; text-align: center;"">
                <img src=""https://i.ibb.co/m6cXxB1/Honest-Auto-Logo.png"" alt=""Honest Auto Logo"" style=""height: 100px; vertical-align: middle; margin-right: 10px;"" />
                <img src=""https://i.ibb.co/6RzfjLM/Honest-Auto-Type.png"" alt=""Honest Auto Type"" style=""height: 100px; vertical-align: middle;"" />
            </td>
        </tr>
    </table>
</body>
</html>

";

                await _emailService.SendEmailAsync(receiverEmail, emailSubject, emailContent, isHtml: true);
            }

            await Clients.User(receiverId).SendAsync("ReceiveMessage", message);
        }

        public async Task<IEnumerable<ChatMessage>> RetrieveMessages(string contactId)
        {
            var currentUserId = Context.UserIdentifier;
            var messages = await _messageService.GetMessagesForConversationAsync(currentUserId, contactId);
            return messages;
        }
    }
}