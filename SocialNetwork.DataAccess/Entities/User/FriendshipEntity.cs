namespace SocialNetwork.DataAccess.Entities
{
    // This entity represents a friendship relationship between two users
    public class FriendshipEntity : BaseEntity
    {
        // Identifier of the first user (one participant in the friendship)
        public Guid User1Id { get; set; }
        public UserEntity User1 { get; set; }

        // Identifier of the second user (another participant in the friendship)
        public Guid User2Id { get; set; }
        public UserEntity User2 { get; set; }

        // Indicates who initiated the friendship request
        public Guid RequestedById { get; set; }
        public UserEntity RequestedBy { get; set; }

        // The date when the friendship was accepted
        public DateTime? AcceptedAt { get; set; }
    }
}
