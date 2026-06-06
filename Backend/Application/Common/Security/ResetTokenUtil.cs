using System.Security.Cryptography;

namespace Application.Common.Security;

/// <summary>
/// Helpers for password-reset tokens. The raw token is emailed to the user;
/// only its SHA-256 hash is ever persisted, so the stored value can't be replayed.
/// </summary>
public static class ResetTokenUtil
{
    /// <summary>Generates a URL-safe, cryptographically random token (256 bits).</summary>
    public static string GenerateRawToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Base64UrlEncode(bytes);
    }

    /// <summary>Returns the hex-encoded SHA-256 hash used for storage and lookup.</summary>
    public static string Hash(string rawToken)
    {
        var hash = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(hash);
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
