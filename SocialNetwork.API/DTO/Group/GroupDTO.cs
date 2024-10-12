namespace SocialNetwork.API.DTO.Group
{
    public record CreateGroupRequest
    {
        public Guid UserId { get; init; }
        public string Name { get; init; }
        public string? Description { get; init; }
    }

    public record CreateGroupByUserRequest
    {
        public string Name { get; init; }
        public string? Description { get; init; }
    }

    public record UpdateGroupRequest
    {
        public Guid UserId { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
    }

    public record UpdateGroupByUserRequest
    {
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
