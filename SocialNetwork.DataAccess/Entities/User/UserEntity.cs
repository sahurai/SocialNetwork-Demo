using SocialNetwork.Core.Enums;
using SocialNetwork.DataAccess.Entities.Auth;
using SocialNetwork.DataAccess.Entities.Group;
using SocialNetwork.DataAccess.Entities.Post;
using SocialNetwork.DataAccess.Entities.User;

namespace SocialNetwork.DataAccess.Entities
{
    // This entity represents a user in the social network
    public class UserEntity : BaseEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public string PasswordHash { get; set; }

        // A collections
        public ICollection<PostEntity> Posts { get; set; } = new List<PostEntity>();
        public ICollection<MessageEntity> SentMessages { get; set; } = new List<MessageEntity>();
        public ICollection<MessageEntity> ReceivedMessages { get; set; } = new List<MessageEntity>();
        public ICollection<FriendshipEntity> Friendships { get; set; } = new List<FriendshipEntity>();
        public ICollection<GroupEntity> Groups { get; set; } = new List<GroupEntity>();
        public ICollection<UserBlockEntity> BlockedUsers { get; set; } = new List<UserBlockEntity>();
        public List<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();
    }
}