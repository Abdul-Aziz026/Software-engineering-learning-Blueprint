using Domain.ValueObjects;
using FluentValidation;

namespace Application.Features.Subscribers.Commands.Subscribe;

public class SubscribeCommandValidator : AbstractValidator<SubscribeCommand>
{
    public SubscribeCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Must(email => Email.TryCreate(email, out _)).WithMessage("A valid email address is required.");
    }
}
