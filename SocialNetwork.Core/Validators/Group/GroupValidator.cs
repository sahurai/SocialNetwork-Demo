using FluentValidation;
using SocialNetwork.Core.Models;
using SocialNetwork.Shared;

namespace SocialNetwork.Core.Validators
{
    /// <summary>
    /// Validator for the Group model.
    /// </summary>
    public class GroupValidator : AbstractValidator<Group>
    {
        public GroupValidator()
        {
            RuleFor(x => x.CreatorId)
                .NotEmpty().WithMessage("CreatorId cannot be empty.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(Constants.MaxGroupNameLength)
                .WithMessage($"Name cannot exceed {Constants.MaxGroupNameLength} characters.");

            RuleFor(x => x.Description)
                .MaximumLength(Constants.MaxGroupDescriptionLength)
                .WithMessage($"Description cannot exceed {Constants.MaxGroupDescriptionLength} characters.");
        }
    }
}
