namespace SocialNetwork.DataAccess.Entities.Group
{
    // This entity represents a block relationship between two users in the context of a group
    public class GroupBlockEntity : BaseEntity
    {
        // The user who initiates the block (admin or manager)
        public Guid BlockerId { get; set; }
        public UserEntity Blocker { get; set; }

        // The user who is being blocked (user)
        public Guid BlockedId { get; set; }
        public UserEntity Blocked { get; set; }

        // The ID of the group where the block applies
        public Guid GroupId { get; set; }
        public GroupEntity Group { get; set; }
    }
}
