using FluentValidation;
using SocialNetwork.Core.Models;
using SocialNetwork.Shared;

namespace SocialNetwork.Core.Validators
{
    /// <summary>
    /// Validator for the User model.
    /// </summary>
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username cannot be empty.")
                .MaximumLength(Constants.MaxUsernameLength)
                .WithMessage($"Username cannot exceed {Constants.MaxUsernameLength} characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email cannot be empty.")
                .MaximumLength(Constants.MaxEmailLength)
                .WithMessage($"Email cannot exceed {Constants.MaxEmailLength} characters.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(x => x.PasswordHash)
                .NotEmpty().WithMessage("PasswordHash cannot be empty.");
        }
    }
}
