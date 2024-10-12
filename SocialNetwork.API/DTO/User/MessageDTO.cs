namespace SocialNetwork.API.DTO.User
{
    public record CreateMessageRequest
    {
        public Guid SenderId { get; init; }
        public Guid ReceiverId { get; init; }
        public string Content { get; init; }
    }

    public record CreateMessageByUserRequest
    {
        public Guid ReceiverId { get; init; }
        public string Content { get; init; }
    }

    public record UpdateMessageRequest
    {
        public Guid SenderId { get; init; }
        public string Content { get; init; }
    }

    public record UpdateMessageByUserRequest
    {
        public string Content { get; init; }
    }

    public record MarkMessagesAsReadRequest
    {
        public Guid UserId { get; init; }
        public List<Guid> MessageIds { get; init; }
    }

    public record MarkMessagesAsReadByUserRequest
    {
        public List<Guid> MessageIds { get; init; }
    }

    public record MessageResponse
    {
        public Guid Id { get; init; }
        public Guid SenderId { get; init; }
        public Guid ReceiverId { get; init; }
        public string Content { get; init; }
        public bool IsRead { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}
