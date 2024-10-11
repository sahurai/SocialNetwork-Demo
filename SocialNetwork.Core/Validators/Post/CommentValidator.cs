using FluentValidation;
using SocialNetwork.Core.Models;
using SocialNetwork.Shared;

namespace SocialNetwork.Core.Validators
{
    /// <summary>
    /// Validator for the Comment model.
    /// </summary>
    public class CommentValidator : AbstractValidator<Comment>
    {
        public CommentValidator()
        {
            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("AuthorId cannot be empty.");

            RuleFor(x => x.PostId)
                .NotEmpty().WithMessage("PostId cannot be empty.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content cannot be empty.")
                .MaximumLength(Constants.MaxCommentContentLength)
                .WithMessage($"Content cannot exceed {Constants.MaxCommentContentLength} characters.");
        }
    }
}
