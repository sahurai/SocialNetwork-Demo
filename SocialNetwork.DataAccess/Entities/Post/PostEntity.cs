using SocialNetwork.DataAccess.Entities.Group;

namespace SocialNetwork.DataAccess.Entities.Post
{
    // This entity represents a post made by a user or in a group
    public class PostEntity : BaseEntity
    {
        // The content of the post
        public string Content { get; set; }

        // The ID of the user who created the post
        public Guid AuthorId { get; set; }

        // The user who created the post
        public UserEntity Author { get; set; }

        // The ID of the group (if the post is in a group)
        public Guid? GroupId { get; set; }

        // The group in which the post was made (if applicable)
        public GroupEntity? Group { get; set; }

        // Edited at field
        public DateTime? EditedAt { get; set; }

        // A collection of comments on this post
        public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();

        // A collection of likes on this post
        public ICollection<LikeEntity> Likes { get; set; } = new List<LikeEntity>();
    }
}
