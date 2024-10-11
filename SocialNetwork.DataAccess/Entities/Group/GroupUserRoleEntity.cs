using SocialNetwork.Core.Enums;
using SocialNetwork.DataAccess.Entities.User;

namespace SocialNetwork.DataAccess.Entities.Group
{
    // This entity represents the role of a user in a group (Admin, Manager, Member)
    public class GroupUserRoleEntity : BaseEntity
    {
        // The ID of the user
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        // The ID of the group
        public Guid GroupId { get; set; }
        public GroupEntity Group { get; set; }

        // The role of the user in this group (Admin, Manager, Member)
        public GroupRole Role { get; set; }
    }
}
