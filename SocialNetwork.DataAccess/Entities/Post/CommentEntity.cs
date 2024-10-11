namespace SocialNetwork.DataAccess.Entities.Post
{
    // This entity represents a comment made on a post
    public class CommentEntity : BaseEntity
    {
        // The content of the comment
        public string Content { get; set; }

        // The ID of the user who made the comment
        public Guid AuthorId { get; set; }

        // The user who made the comment
        public UserEntity Author { get; set; }

        // The ID of the post that the comment is associated with
        public Guid PostId { get; set; }

        // The post that the comment is associated with
        public PostEntity Post { get; set; }

        // The date and time when the comment was edited (nullable)
        public DateTime? EditedAt { get; set; }
    }
}
