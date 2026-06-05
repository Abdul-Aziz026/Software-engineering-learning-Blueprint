
using Application.Features.Auth.Commands.UpdateProfile;

namespace Application.Features.Auth.DTOs;

public class UpdateProfileRequestDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }

    public UpdateProfileCommand ToUpdateProfileCommand(string userId)
    {
        return new UpdateProfileCommand
        {
            UserId = userId,
            Username = Username,
            Email = Email,
            CurrentPassword = CurrentPassword,
            NewPassword = NewPassword
        };
    }
}
