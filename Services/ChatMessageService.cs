using Microsoft.EntityFrameworkCore;
using HonestAuto.Models;
using HonestAuto.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace HonestAuto.Services
{
    public class ChatMessageService
    {
        private readonly MarketplaceContext _context;
        private readonly UserManager<User> _userManager; // Define it here

        public ChatMessageService(MarketplaceContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager; // Initialize it in the constructor
        }

        // This method saves a chat message to the database
        public async Task SaveMessageAsync(ChatMessage message)
        {
            // Add the chat message to the context
            _context.ChatMessages.Add(message);

            // Save the changes to the database asynchronously
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatViewModel>> GetConversationsAsync(string currentUserId)
        {
            var conversations = await _context.ChatMessages
                .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                .GroupBy(m => m.ReceiverId)
                .ToListAsync();

            var conversationViewModels = new List<ChatViewModel>();

            foreach (var conversation in conversations)
            {
                var receiverId = conversation.Key;

                // Check if the receiver ID is the same as the current user's ID
                if (receiverId == currentUserId)
                {
                    // Skip the current user's conversation
                    continue;
                }

                var receiver = await _userManager.FindByIdAsync(receiverId);

                if (receiver != null)
                {
                    var receiverUsername = receiver.UserName;

                    var lastMessage = conversation.OrderByDescending(m => m.DateSent).FirstOrDefault();
                    var content = lastMessage?.Content;

                    conversationViewModels.Add(new ChatViewModel
                    {
                        ReceiverId = receiverId,
                        ReceiverUsername = receiverUsername,
                        Content = content
                    });
                }
                else
                {
                    // Handle the case where the user with the given ID is not found.
                    // You can skip this conversation or handle it as needed.
                }
            }

            return conversationViewModels;
        }

        // Method to fetch the username for a given user ID
        public async Task<string> FetchUsernameForIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null ? user.UserName : "User Not Found";
        }

        // This method retrieves messages for a specific conversation between two users
        public async Task<IEnumerable<ChatMessage>> GetMessagesForConversationAsync(string senderId, string receiverId)
        {
            // Retrieve chat messages from the database and include the Sender user data
            return await _context.ChatMessages
                .Include(m => m.Sender) // Include the Sender user data
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) || // Messages from sender to receiver
                            (m.SenderId == receiverId && m.ReceiverId == senderId)) // Messages from receiver to sender
                .OrderBy(m => m.DateSent) // Order messages by timestamp
                .ToListAsync(); // Execute the query and return a list of chat messages
        }
    }
}