using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using HonestAuto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HonestAuto.Services;

namespace HonestAuto.Hubs

{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ChatMessageService _messageService;

        public ChatHub(ChatMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task SendMessage(string receiverId, string messageContent)
        {
            var senderId = Context.UserIdentifier; // Get the sender's user ID from the connection context
            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                DateSent = DateTime.UtcNow,
                Content = messageContent
            };

            // Save the message to the database
            await _messageService.SaveMessageAsync(message);

            // Broadcast the message to the receiver
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