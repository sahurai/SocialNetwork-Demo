using FluentValidation;
using SocialNetwork.Core.Models;

namespace SocialNetwork.Core.Validators
{
    /// <summary>
    /// Validator for the UserBlock model.
    /// </summary>
    public class UserBlockValidator : AbstractValidator<UserBlock>
    {
        public UserBlockValidator()
        {
            RuleFor(x => x.BlockerId)
                .NotEmpty().WithMessage("BlockerId cannot be empty.");

            RuleFor(x => x.BlockedId)
                .NotEmpty().WithMessage("BlockedId cannot be empty.")
                .NotEqual(x => x.BlockerId).WithMessage("BlockedId cannot be the same as BlockerId.");
        }
    }
}
