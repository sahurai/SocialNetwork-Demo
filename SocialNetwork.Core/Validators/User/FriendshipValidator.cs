using FluentValidation;
using SocialNetwork.Core.Models;

namespace SocialNetwork.Core.Validators
{
    /// <summary>
    /// Validator for the Friendship model.
    /// </summary>
    public class FriendshipValidator : AbstractValidator<Friendship>
    {
        public FriendshipValidator()
        {
            RuleFor(x => x.User1Id)
                .NotEmpty().WithMessage("User1Id cannot be empty.");

            RuleFor(x => x.User2Id)
                .NotEmpty().WithMessage("User2Id cannot be empty.")
                .NotEqual(x => x.User1Id).WithMessage("User2Id cannot be the same as User1Id.");

            RuleFor(x => x.RequestedById)
                .NotEmpty().WithMessage("RequestedById cannot be empty.")
                .Must((model, requestedById) => requestedById == model.User1Id || requestedById == model.User2Id)
                .WithMessage("RequestedById must be either User1Id or User2Id.");
        }
    }
}
