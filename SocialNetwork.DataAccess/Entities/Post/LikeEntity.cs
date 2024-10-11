namespace SocialNetwork.DataAccess.Entities.Post
{
    // This entity represents a like on a post or comment
    public class LikeEntity : BaseEntity
    {
        // The ID of the user who liked the post or comment
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }

        // The ID of the post that was liked (nullable if it's a comment like)
        public Guid? PostId { get; set; }
        public PostEntity? Post { get; set; }

        // The ID of the comment that was liked (nullable if it's a post like)
        public Guid? CommentId { get; set; }
        public CommentEntity? Comment { get; set; }
    }
}
