namespace SocialNetwork.API.DTO.Post
{
    public record CreateCommentRequest
    {
        public Guid UserId { get; init; }
        public Guid PostId { get; init; }
        public string Content { get; init; }
    }

    public record CreateCommentByUserRequest
    {
        public Guid PostId { get; init; }
        public string Content { get; init; }
    }

    public record UpdateCommentRequest
    {
        public Guid UserId { get; init; }
        public string Content { get; init; }
    }

    public record UpdateCommentByUserRequest
    {
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
