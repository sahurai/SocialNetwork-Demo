using FluentValidation;
using SocialNetwork.Core.Models;

namespace SocialNetwork.Core.Validators
{
    /// <summary>
    /// Validator for the GroupUserRole model.
    /// </summary>
    public class GroupUserRoleValidator : AbstractValidator<GroupUserRole>
    {
        public GroupUserRoleValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId cannot be empty.");

            RuleFor(x => x.GroupId)
                .NotEmpty().WithMessage("GroupId cannot be empty.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role cannot be empty.")
                .IsInEnum().WithMessage("Role must be a valid GroupRole.");
        }
    }
}
