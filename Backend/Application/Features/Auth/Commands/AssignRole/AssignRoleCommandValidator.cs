using Domain.Enums;
using FluentValidation;

namespace Application.Features.Auth.Commands.AssignRole;

public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleCommandValidator()
    {
        RuleFor(x => x.TargetUserId).NotEmpty().WithMessage("Target user id is required.");

        // Only User/Admin are assignable here; SuperAdmin is bootstrapped from config, never granted via the API.
        RuleFor(x => x.Role)
            .Must(r => r is UserRole.User or UserRole.Admin)
            .WithMessage("Role must be either 'User' or 'Admin'.");
    }
}
