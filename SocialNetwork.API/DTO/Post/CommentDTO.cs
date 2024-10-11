namespace SocialNetwork.Api.DTOs
{
    public record CreateCommentRequest
    {
        public Guid RequestingUserId { get; init; }
        public Guid PostId { get; init; }
        public string Content { get; init; }
    }

    public record UpdateCommentRequest
    {
        public Guid RequestingUserId { get; init; }
        public string Content { get; init; }
    }

    public record CommentResponse
    {
        public Guid Id { get; init; }
        public Guid AuthorId { get; init; }
        public Guid PostId { get; init; }
        public string Content { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}
