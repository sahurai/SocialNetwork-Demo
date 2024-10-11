namespace SocialNetwork.Api.DTOs
{
    public record CreateGroupRequest
    {
        public Guid RequestingUserId { get; init; }
        public string Name { get; init; }
        public string? Description { get; init; }
    }

    public record UpdateGroupRequest
    {
        public Guid RequestingUserId { get; init; }
        public Guid? CreatorId { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
    }

    public record GroupResponse
    {
        public Guid Id { get; init; }
        public Guid CreatorId { get; init; }
        public string Name { get; init; }
        public string? Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}
