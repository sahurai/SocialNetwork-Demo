using SocialNetwork.DataAccess.Entities.Post;

namespace SocialNetwork.DataAccess.Entities.Group
{
    // This entity represents a group that users can join
    public class GroupEntity : BaseEntity
    {
        // The name of the group
        public string Name { get; set; }

        // A description of the group
        public string? Description { get; set; }

        // The ID of the creator of the group (admin)
        public Guid CreatorId { get; set; }

        // The creator of the group (admin)
        public UserEntity Creator { get; set; }

        // A collection of roles (admin, manager, member) for the users in the group
        public ICollection<GroupUserRoleEntity> Members { get; set; } = new List<GroupUserRoleEntity>();

        // A collection of posts made in the group
        public ICollection<PostEntity> Posts { get; set; } = new List<PostEntity>();
    }
}
