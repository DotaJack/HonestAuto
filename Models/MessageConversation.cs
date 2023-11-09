namespace HonestAuto.Models
{
    public class MessageConversation
    {
        public int MessageConversationID { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public int UserID1 { get; set; }
        public int UserID2 { get; set; }
    }
}