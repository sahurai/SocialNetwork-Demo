using SocialNetwork.Core.Validators;

namespace SocialNetwork.Core.Models.Auth
{
    public class RefreshToken : BaseClass
    {
        public string Token { get; }
        public Guid UserId { get; }
        public DateTime ExpiryDate { get; }
        public string UserAgent { get; }
        public string IpAddress { get; }

        // Private constructor to create a Refresh Token instance
        private RefreshToken(Guid id, string token, Guid userId, DateTime expiryDate, string userAgent, string ipAddress)
        {
            Id = id;
            Token = token;
            UserId = userId;
            ExpiryDate = expiryDate;
            UserAgent = userAgent;
            IpAddress = ipAddress;
        }

        // Static method to create a new Refresh Token with validation
        public static (RefreshToken? Token, string Error) Create(
            string token,
            Guid userId,
            DateTime expiryDate,
            string userAgent,
            string ipAddress)
        {
            var refreshToken = new RefreshToken(Guid.NewGuid(), token, userId, expiryDate, userAgent, ipAddress);

            // Validate
            var validator = new RefreshTokenValidator();
            var validationResult = validator.Validate(refreshToken);

            if (!validationResult.IsValid)
            {
                string error = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (null, error);
            }

            return (refreshToken, string.Empty);
        }

        // Method to recreate an instance from database data
        public static (RefreshToken? Token, string Error) CreateFromDb(
            Guid id,
            string token,
            Guid userId,
            DateTime expiryDate,
            string userAgent,
            string ipAddress,
            DateTime createdAt,
            DateTime updatedAt)
        {
            var refreshToken = new RefreshToken(id, token, userId, expiryDate, userAgent, ipAddress)
            {
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            return (refreshToken, string.Empty);
        }
    }
}
