namespace Application.Settings;

public class PasswordResetOptions
{
    public int TokenExpiryMinutes { get; set; } = 60;

    /// <summary>Base URL of the frontend; the reset link is "{FrontendUrl}/reset-password?token=...".</summary>
    public string FrontendUrl { get; set; } = string.Empty;
}
