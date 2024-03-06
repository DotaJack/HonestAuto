using HonestAuto.Models;
using HonestAuto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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
                var senderEmail = sender?.Email;
                var emailSubject = "New Chat Message";
                var emailContent = $"You have received a new message from {senderEmail ?? "a user"}: {messageContent}";
                await _emailService.SendEmailAsync(receiverEmail, emailSubject, emailContent);
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