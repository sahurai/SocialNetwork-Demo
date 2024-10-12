namespace SocialNetwork.API.DTO.User
{
    public record RegisterRequest
    {
        public string Username { get; init; }
        public string Email {  get; init; }
        public string Password { get; init; }
    }

    public record LoginRequest
    {
        public string Email { get; init; }
        public string Password { get; init; }
    }

    public record RevokeTokenRequestByUserAgent
    {
        public string UserAgent { get; init; }
    }
}
