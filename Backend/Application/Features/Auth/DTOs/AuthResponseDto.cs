using Domain.Entities;

namespace Application.Features.Auth.DTOs;

public class AuthResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public static AuthResponseDto FromUser(User user) => new()
    {
        UserId = user.Id,
        Username = user.Username,
        Email = user.Email
    };
}
