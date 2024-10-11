﻿namespace SocialNetwork.Api.DTOs
{
    public record CreateUserRequest
    {
        public string Username { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
    }

    public record UpdateUserRequest
    {
        public string? Username { get; init; }
        public string? Email { get; init; }
        public string? Password { get; init; }
    }

    public record UserResponse
    {
        public Guid Id { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}