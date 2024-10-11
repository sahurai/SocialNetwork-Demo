using SocialNetwork.Core.Enums;

namespace SocialNetwork.Api.DTOs
{
    public record CreateGroupUserRoleRequest
    {
        public Guid GroupId { get; init; }
        public Guid RequestingUserId { get; init; }
        public GroupRole Role { get; init; }
    }

    public record UpdateGroupUserRoleRequest
    {
        public Guid RequestingUserId { get; init; }
        public GroupRole Role { get; init; }
    }

    public record GroupUserRoleResponse
    {
        public Guid Id { get; init; }
        public Guid GroupId { get; init; }
        public Guid UserId { get; init; }
        public GroupRole Role { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}
