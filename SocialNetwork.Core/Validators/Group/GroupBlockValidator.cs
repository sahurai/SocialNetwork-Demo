using FluentValidation;
using SocialNetwork.Core.Models;

namespace SocialNetwork.Core.Validators
{
    /// <summary>
    /// Validator for the GroupBlock model.
    /// </summary>
    public class GroupBlockValidator : AbstractValidator<GroupBlock>
    {
        public GroupBlockValidator()
        {
            RuleFor(x => x.BlockerId)
                .NotEmpty().WithMessage("BlockerId cannot be empty.");

            RuleFor(x => x.BlockedId)
                .NotEmpty().WithMessage("BlockedId cannot be empty.")
                .NotEqual(x => x.BlockerId).WithMessage("BlockedId cannot be the same as BlockerId.");

            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("GroupId cannot be empty.");
        }
    }
}
