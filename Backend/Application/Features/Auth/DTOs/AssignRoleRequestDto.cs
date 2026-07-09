using Application.Features.Auth.Commands.AssignRole;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Features.Auth.DTOs;

public class AssignRoleRequestDto
{
    // Role name sent by the client, e.g. "Admin" or "User" (case-insensitive).
    public string Role { get; set; } = string.Empty;

    public AssignRoleCommand ToAssignRoleCommand(string actingUserId, string targetUserId) => new()
    {
        ActingUserId = actingUserId,
        TargetUserId = targetUserId,
        Role = Enum.TryParse<UserRole>(Role, ignoreCase: true, out var role)
            ? role
            : throw new ValidationException($"Invalid role '{Role}'.")
    };
}
