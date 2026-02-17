using System.Threading;

namespace Application.Common.Helper;

public static class TellMe
{
    private static readonly AsyncLocal<CurrentUserContext?> _current = new();

    // 🔹 Exposed read-only access
    public static string? UserId => _current.Value?.UserId;
    public static string? Email => _current.Value?.Email;
    public static List<string>? Roles => _current.Value?.Roles;
    public static string? Jti => _current.Value?.Jti;

    public static string IpAddress { get; set; } = string.Empty;

    public static void SetCurrentUserContext(CurrentUserContext holder)
    {
        _current.Value = holder;
    }

    public static CurrentUserContext? GetCurrentUserContext() => _current.Value;

    public static void ClearCurrentUserContext()
    {
        _current.Value = null;
    }
}

public sealed class CurrentUserContext
{
    public string? UserId { get; init; }
    public string? Email { get; init; }
    public List<string> Roles { get; init; } = new();
    public string? Jti { get; init; }
}
