using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HonestAuto.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; } // Primary key for the chat message

        [Required]
        public string SenderId { get; set; } // ID of the user who sent the message

        [Required]
        public string ReceiverId { get; set; } // ID of the user who will receive the message

        [Required]
        public DateTime DateSent { get; set; } // Timestamp indicating when the message was sent

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } // The content of the chat message, limited to 1000 characters

        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; } // Navigation property representing the sender of the message

        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; } // Navigation property representing the receiver of the message
    }
}