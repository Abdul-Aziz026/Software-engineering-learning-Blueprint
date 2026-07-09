using Domain.Entities;

namespace Application.Features.Auth.DTOs;

public class AuthResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Role name (e.g. "User", "Admin", "SuperAdmin"); the frontend uses it to gate admin UI.
    public string Role { get; set; } = string.Empty;

    // JWT access token + refresh token. Empty on responses that don't mint tokens (e.g. profile reads).
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }

    public static AuthResponseDto FromUser(User user) => new()
    {
        UserId = user.Id,
        Username = user.Username,
        Email = user.Email.Value,
        Role = user.Role.ToString()
    };

    // Overlays freshly minted tokens onto a user response (used by signup / login / refresh).
    public AuthResponseDto WithTokens(string token, string refreshToken, DateTime expiresAt)
    {
        Token = token;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        return this;
    }
}
