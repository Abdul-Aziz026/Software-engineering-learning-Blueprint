using Application.Common.Interfaces.Services;
using Contracts.Messaging;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

/// <summary>Renders and delivers a password-reset email. Exceptions escape to trigger MassTransit retry/DLQ.</summary>
public class SendPasswordResetEmailConsumer : IConsumer<SendPasswordResetEmail>
{
    private readonly IEmailSender _emailSender;

    public SendPasswordResetEmailConsumer(IEmailSender emailSender) => _emailSender = emailSender;

    public Task Consume(ConsumeContext<SendPasswordResetEmail> context)
    {
        var m = context.Message;
        return _emailSender.SendAsync(m.ToEmail, "Reset your password", BuildEmailBody(m), context.CancellationToken);
    }

    private static string BuildEmailBody(SendPasswordResetEmail m) => $@"
        <div style=""font-family:Arial,sans-serif;font-size:15px;color:#1a1a1a;line-height:1.6"">
            <p>Hi {System.Net.WebUtility.HtmlEncode(m.Username)},</p>
            <p>We received a request to reset your password. Click the button below to choose a new one:</p>
            <p style=""margin:24px 0"">
                <a href=""{m.ResetLink}""
                   style=""background:#2563eb;color:#fff;padding:12px 22px;border-radius:8px;text-decoration:none;display:inline-block"">
                    Reset password
                </a>
            </p>
            <p style=""font-size:13px;color:#666"">
                This link expires in {m.ExpiryMinutes} minutes. If you didn't request a reset, you can safely ignore this email.
            </p>
            <p style=""font-size:13px;color:#666"">If the button doesn't work, paste this link into your browser:<br>{m.ResetLink}</p>
        </div>";
}
