namespace SocialNetwork.DataAccess.Entities.User
{
    // This entity represents a block relationship between two users
    public class UserBlockEntity : BaseEntity
    {
        // Identifier of the user who initiates the block (blocker)
        public Guid BlockerId { get; set; }
        public UserEntity Blocker { get; set; }

        // Identifier of the user who is being blocked (blocked)
        public Guid BlockedId { get; set; }
        public UserEntity Blocked { get; set; }
    }
}
