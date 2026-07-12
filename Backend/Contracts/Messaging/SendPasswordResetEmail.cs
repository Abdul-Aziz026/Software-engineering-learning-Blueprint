namespace Contracts.Messaging;

/// <summary>Command: send a password-reset email. Rendered and delivered by its consumer, off the request thread.</summary>
public record SendPasswordResetEmail(
    string ToEmail,
    string Username,
    string ResetLink,
    int ExpiryMinutes);
