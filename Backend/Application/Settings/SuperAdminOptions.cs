namespace Application.Settings;

/// <summary>
/// Bootstraps the single privileged account. A user whose email matches <see cref="Email"/>
/// is elevated to SuperAdmin on signup/login (idempotent) — no manual DB edit needed.
/// The SuperAdmin then assigns the Admin role to other users.
/// </summary>
public class SuperAdminOptions
{
    public string Email { get; set; } = string.Empty;
}
