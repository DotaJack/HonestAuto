namespace HonestAuto.Models
{
    public class ChatViewModel
    {
        // Represents a collection of chat messages
        public IEnumerable<ChatMessage> Messages { get; set; }

        // Represents the ID of the chat receiver
        public string ReceiverId { get; set; }

        // Represents the content of a new message being composed
        public string Content { get; set; }
    }
}