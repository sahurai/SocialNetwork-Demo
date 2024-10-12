namespace SocialNetwork.API.DTO.Group
{
    public record CreateGroupBlockRequest
    {
        public Guid GroupId { get; init; }
        public Guid UserId { get; init; }
        public Guid BlockedId { get; init; }
    }

    public record CreateGroupBlockByUserRequest
    {
        public Guid GroupId { get; init; }
        public Guid BlockedId { get; init; }
    }

    public record GroupBlockResponse
    {
        public Guid Id { get; init; }
        public Guid GroupId { get; init; }
        public Guid BlockerId { get; init; }
        public Guid BlockedId { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}
