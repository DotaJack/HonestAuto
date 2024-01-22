namespace HonestAuto.Models
{
    public class ChatViewModel
    {
        public IEnumerable<ChatMessage> Messages { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }
}