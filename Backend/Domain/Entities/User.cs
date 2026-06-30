using Domain.Exceptions;
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

    /// <summary>
    /// The sanctioned way application code creates a User. Enforces the
    /// creation invariants HERE in the Domain — a normalized username and the
    /// presence of credentials — so no handler can build an invalid User.
    /// </summary>
    public static User Register(string username, Email email, string passwordHash, string passwordSalt)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ValidationException("Username is required.");
        if (email is null)
            throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(passwordSalt))
            throw new ValidationException("Password credentials are required.");

        return new User
        {
            Username = username.Trim().ToLowerInvariant(),
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };
    }
}
