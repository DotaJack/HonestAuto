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

        public async Task SaveMessageAsync(ChatMessage message)
        {
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesForConversationAsync(string senderId, string receiverId)
        {
            return await _context.ChatMessages
                .Include(m => m.Sender) // Include the Sender user data
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.DateSent)
                .ToListAsync();
        }

    }
}