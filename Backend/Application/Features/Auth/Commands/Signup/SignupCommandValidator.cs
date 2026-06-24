using Domain.ValueObjects;
using FluentValidation;

namespace Application.Features.Auth.Commands.Signup;

// Declarative validation rules for SignupCommand. AddValidatorsFromAssembly
// discovers this automatically, and ValidationBehavior runs it before
// SignupCommandHandler — so the handler only ever sees a shape-valid request.
public class SignupCommandValidator : AbstractValidator<SignupCommand>
{
    public SignupCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Username must be 50 characters or fewer.");

        // Reuse the Domain rule instead of duplicating a regex here.
        // The Email value object is the single source of truth for "what is a valid email".
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Must(BeAValidEmail).WithMessage("'{PropertyValue}' is not a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(4).WithMessage("Password must be at least 4 characters.");
    }

    private static bool BeAValidEmail(string email) => Email.TryCreate(email, out _);
}
