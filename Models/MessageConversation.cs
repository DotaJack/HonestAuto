namespace HonestAuto.Models
{
    public class MessageConversation
    {
        public int ConversationID { get; set; }
        public string Content { get; set; }
        public DateTime TimeStamp { get; set; }
        public int SellerID { get; set; }
        public int BuyerID { get; set; }
    }
}