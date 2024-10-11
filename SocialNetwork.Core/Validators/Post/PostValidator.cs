using FluentValidation;
using SocialNetwork.Core.Models;
using SocialNetwork.Shared;

namespace SocialNetwork.Core.Validators
{
    /// <summary>
    /// Validator for the Post model.
    /// </summary>
    public class PostValidator : AbstractValidator<Post>
    {
        public PostValidator()
        {
            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("AuthorId cannot be empty.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content cannot be empty.")
                .MaximumLength(Constants.MaxPostContentLength)
                .WithMessage($"Content cannot exceed {Constants.MaxPostContentLength} characters.");
        }
    }
}
