using FluentValidation;
using SocialNetwork.Core.Models;

namespace SocialNetwork.Core.Validators
{
    /// <summary>
    /// Validator for the Like model.
    /// </summary>
    public class LikeValidator : AbstractValidator<Like>
    {
        public LikeValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId cannot be empty.");

            RuleFor(x => x)
                .Must(x => x.PostId.HasValue ^ x.CommentId.HasValue)
                .WithMessage("Either PostId or CommentId must be set, but not both.");
        }
    }
}
