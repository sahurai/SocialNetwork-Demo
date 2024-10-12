namespace SocialNetwork.API.DTO.Post
{
    public record CreateLikeRequest
    {
        public Guid UserId { get; init; }
        public Guid? PostId { get; init; }
        public Guid? CommentId { get; init; }
    }

    public record CreateLikeByUserRequest
    {
        public Guid? PostId { get; init; }
        public Guid? CommentId { get; init; }
    }

    public record LikeResponse
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public Guid? PostId { get; init; }
        public Guid? CommentId { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
