using Microsoft.EntityFrameworkCore;
using HonestAuto.Models;
using HonestAuto.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonestAuto.Services
{
    public class ChatMessageService
    {
        private readonly MarketplaceContext _context;

        public ChatMessageService(MarketplaceContext context)
        {
            _context = context;
        }

        // This method saves a chat message to the database
        public async Task SaveMessageAsync(ChatMessage message)
        {
            // Add the chat message to the context
            _context.ChatMessages.Add(message);

            // Save the changes to the database asynchronously
            await _context.SaveChangesAsync();
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