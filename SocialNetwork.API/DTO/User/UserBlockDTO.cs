namespace SocialNetwork.API.DTO.User
{
    public record CreateUserBlockRequest
    {
        public Guid BlockerId { get; init; }
        public Guid BlockedId { get; init; }
    }

    public record CreateUserBlockByUserRequest
    {
        public Guid BlockedId { get; init; }
    }

    public record UserBlockResponse
    {
        public Guid Id { get; init; }
        public Guid BlockerId { get; init; }
        public Guid BlockedId { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}
