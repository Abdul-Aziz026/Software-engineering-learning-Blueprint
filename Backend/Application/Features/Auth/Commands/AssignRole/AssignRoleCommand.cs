using Application.Features.Auth.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Features.Auth.Commands.AssignRole;

public class AssignRoleCommand : IRequest<AuthResponseDto>
{
    // The user performing the assignment (from the X-User-Id header). Must be a SuperAdmin.
    public string ActingUserId { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
