using Domain.ValueObjects;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public Email Email { get; set; } = null!;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;

    // Password recovery: we store only the SHA-256 hash of the reset token,
    // never the token itself, so a DB leak can't be used to reset passwords.
    public string? PasswordResetTokenHash { get; set; }
    public DateTime? PasswordResetTokenExpiresAt { get; set; }
}
