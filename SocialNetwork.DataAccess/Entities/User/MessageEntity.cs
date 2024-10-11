namespace SocialNetwork.DataAccess.Entities.User
{
    // This entity represents a private message between two users
    public class MessageEntity : BaseEntity
    {
        // The content of the message
        public string Content { get; set; }

        // The ID of the user who sent the message
        public Guid SenderId { get; set; }
        public UserEntity Sender { get; set; }

        // The ID of the user who received the message
        public Guid ReceiverId { get; set; }
        public UserEntity Receiver { get; set; }

        // The date and time when the message was edited (nullable)
        public DateTime? EditedAt { get; set; }

        // Whether the message has been read by the recipient
        public bool IsRead { get; set; }
    }
}
