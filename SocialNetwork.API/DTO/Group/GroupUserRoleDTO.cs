using SocialNetwork.Core.Enums;

namespace SocialNetwork.API.DTO.Group
{
    public record CreateGroupUserRoleRequest
    {
        public Guid GroupId { get; init; }
        public Guid UserId { get; init; }
    }

    public record CreateGroupUserRoleByUserRequest
    {
        public Guid GroupId { get; init; }
        public Guid UserId { get; init; }
    }

    public record UpdateGroupUserRoleRequest
    {
        public Guid UserId { get; init; }
        public GroupRole Role { get; init; }
    }

    public record UpdateGroupUserRoleByUserRequest
    {
        public Guid GroupId { get; init; }
        public Guid UserId { get; init; }
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
