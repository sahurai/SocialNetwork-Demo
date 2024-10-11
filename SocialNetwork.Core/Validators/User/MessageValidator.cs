using FluentValidation;
using SocialNetwork.Core.Models;
using SocialNetwork.Shared;

namespace SocialNetwork.Core.Validators
{
    /// <summary>
    /// Validator for the Message model.
    /// </summary>
    public class MessageValidator : AbstractValidator<Message>
    {
        public MessageValidator()
        {
            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("SenderId cannot be empty.");

            RuleFor(x => x.ReceiverId)
                .NotEmpty().WithMessage("ReceiverId cannot be empty.")
                .NotEqual(x => x.SenderId).WithMessage("ReceiverId cannot be the same as SenderId.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content cannot be empty.")
                .MaximumLength(Constants.MaxMessageContentLength)
                .WithMessage($"Content cannot exceed {Constants.MaxMessageContentLength} characters.");
        }
    }
}
