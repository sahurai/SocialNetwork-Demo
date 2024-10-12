namespace SocialNetwork.API.DTO.User
{
    public record CreateFriendshipRequest
    {
        public Guid User1Id { get; init; }
        public Guid User2Id { get; init; }
    }

    public record CreateFriendshipByUserRequest
    {
        public Guid newFriendId { get; init; }
    }

    public record FriendshipResponse
    {
        public Guid Id { get; init; }
        public Guid User1Id { get; init; }
        public Guid User2Id { get; init; }
        public bool IsAccepted { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
