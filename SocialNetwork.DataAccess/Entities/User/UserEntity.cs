using SocialNetwork.DataAccess.Entities.Group;
using SocialNetwork.DataAccess.Entities.Post;
using SocialNetwork.DataAccess.Entities.User;

namespace SocialNetwork.DataAccess.Entities
{
    // This entity represents a user in the social network
    public class UserEntity : BaseEntity
    {
        // The username chosen by the user
        public string Username { get; set; }

        // The user's email address
        public string Email { get; set; }

        // The user's password (stored securely - hashed)
        public string PasswordHash { get; set; }

        // A collection of posts created by the user
        public ICollection<PostEntity> Posts { get; set; } = new List<PostEntity>();

        // A collection of messages sent by the user
        public ICollection<MessageEntity> SentMessages { get; set; } = new List<MessageEntity>();

        // A collection of messages received by the user
        public ICollection<MessageEntity> ReceivedMessages { get; set; } = new List<MessageEntity>();

        // A collection of friendships involving this user
        public ICollection<FriendshipEntity> Friendships { get; set; } = new List<FriendshipEntity>();

        // A collection of groups the user is a member of
        public ICollection<GroupEntity> Groups { get; set; } = new List<GroupEntity>();

        // A collection of users that this user has blocked
        public ICollection<UserBlockEntity> BlockedUsers { get; set; } = new List<UserBlockEntity>();
    }
}