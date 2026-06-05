
using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileCommand : IRequest<AuthResponseDto>
{
    public string UserId { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}
