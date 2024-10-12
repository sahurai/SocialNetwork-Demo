using SocialNetwork.Core.Enums;

namespace SocialNetwork.API.DTO.User
{
    public record CreateUserRequest
    {
        public string Username { get; init; }
        public string Email { get; init; }
        public UserRole Role { get; init; }
        public string Password { get; init; }
    }

    public record UpdateUserRequest
    {
        public string? Username { get; init; }
        public string? Email { get; init; }
        public string? Password { get; init; }
    }

    public record UserResponseWithAdditionalInfo
    {
        public Guid Id { get; init; }
        public string Username { get; init; }
        public UserRole Role { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }

    public record UserResponse
    {
        public Guid Id { get; init; }
        public string Username { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
