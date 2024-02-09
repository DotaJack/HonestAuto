using HonestAuto.Models;
using HonestAuto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

// Source:https://www.youtube.com/watch?v=RUZLIh4Vo20
// Source 2: https://learn.microsoft.com/en-us/aspnet/signalr/overview/getting-started/tutorial-getting-started-with-signalr
namespace HonestAuto.Hubs
{
    // Authorize attribute ensures that only authenticated users can access this hub
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ChatMessageService _messageService;

        public ChatHub(ChatMessageService messageService)
        {
            _messageService = messageService;
        }

        // This method is called when a user sends a message
        public async Task SendMessage(string receiverId, string messageContent)
        {
            // Get the sender's user ID from the connection context
            var senderId = Context.UserIdentifier;

            // Create a new ChatMessage instance with sender, receiver, timestamp, and content
            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                DateSent = DateTime.UtcNow,
                Content = messageContent
            };

            // Save the message to the database
            await _messageService.SaveMessageAsync(message);

            // Broadcast the message to the specified receiver
            await Clients.User(receiverId).SendAsync("ReceiveMessage", message);
        }

        // This method retrieves messages for a specific conversation
        public async Task<IEnumerable<ChatMessage>> RetrieveMessages(string contactId)
        {
            // Get the current user's ID from the connection context
            var currentUserId = Context.UserIdentifier;

            // Fetch messages for the conversation between the current user and the specified contact
            var messages = await _messageService.GetMessagesForConversationAsync(currentUserId, contactId);

            return messages;
        }
    }
}