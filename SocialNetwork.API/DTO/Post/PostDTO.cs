namespace SocialNetwork.API.DTO.Post
{
    public record CreatePostRequest
    {
        public Guid UserId { get; init; }
        public string Content { get; init; }
        public Guid? GroupId { get; init; }
    }

    public record CreatePostByUserRequest
    {
        public string Content { get; init; }
        public Guid? GroupId { get; init; }
    }

    public record UpdatePostRequest
    {
        public Guid UserId { get; init; }
        public string Content { get; init; }
    }

    public record UpdatePostByUserRequest
    {
        public string Content { get; init; }
    }

    public record PostResponse
    {
        public Guid Id { get; init; }
        public Guid AuthorId { get; init; }
        public Guid? GroupId { get; init; }
        public string Content { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}
