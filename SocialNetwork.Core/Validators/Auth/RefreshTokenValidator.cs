using FluentValidation;
using SocialNetwork.Core.Models.Auth;

namespace SocialNetwork.Core.Validators
{
    public class RefreshTokenValidator : AbstractValidator<RefreshToken>
    {
        public RefreshTokenValidator()
        {
            RuleFor(rt => rt.Token)
                .NotEmpty().WithMessage("Token cannot be empty.");

            RuleFor(rt => rt.UserId)
                .NotEmpty().WithMessage("UserId cannot be empty.")
                .NotEqual(Guid.Empty).WithMessage("UserId cannot be Guid.Empty.");

            RuleFor(rt => rt.ExpiryDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.");

            RuleFor(rt => rt.UserAgent)
                .NotEmpty().WithMessage("UserAgent cannot be empty.");

            RuleFor(rt => rt.IpAddress)
                .NotEmpty().WithMessage("IpAddress cannot be empty.");
        }
    }
}
