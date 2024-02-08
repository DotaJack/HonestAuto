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
            var conversationViewModels = new List<ChatViewModel>();

            // Retrieve all messages involving the current user
            var messages = await _context.ChatMessages
                .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                .ToListAsync();

            // Group messages by the other user's ID
            var groupedMessages = messages
                .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId);

            foreach (var group in groupedMessages)
            {
                var otherUserId = group.Key;

                // Skip the current user's own ID
                if (otherUserId == currentUserId)
                {
                    continue;
                }

                // Get the other user's information
                var otherUser = await _userManager.FindByIdAsync(otherUserId);

                if (otherUser != null)
                {
                    var receiverUsername = otherUser.UserName;

                    // Get the last message in the conversation
                    var lastMessage = group.OrderByDescending(m => m.DateSent).FirstOrDefault();
                    var content = lastMessage?.Content;

                    conversationViewModels.Add(new ChatViewModel
                    {
                        ReceiverId = otherUserId,
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