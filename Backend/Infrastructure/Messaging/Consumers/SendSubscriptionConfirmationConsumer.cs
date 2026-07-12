using Application.Common.Interfaces.Services;
using Contracts.Messaging;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

/// <summary>Renders and delivers a subscription-confirmation email. Exceptions escape to trigger MassTransit retry/DLQ.</summary>
public class SendSubscriptionConfirmationConsumer : IConsumer<SendSubscriptionConfirmation>
{
    private readonly IEmailSender _emailSender;

    public SendSubscriptionConfirmationConsumer(IEmailSender emailSender) => _emailSender = emailSender;

    public Task Consume(ConsumeContext<SendSubscriptionConfirmation> context)
    {
        var m = context.Message;
        return _emailSender.SendAsync(
            m.ToEmail, "Confirm your newsletter subscription", BuildEmailBody(m), context.CancellationToken);
    }

    private static string BuildEmailBody(SendSubscriptionConfirmation m) => $@"
        <div style=""font-family:Arial,sans-serif;font-size:15px;color:#1a1a1a;line-height:1.6"">
            <p>Thanks for subscribing!</p>
            <p>Please confirm your email address to start receiving our newsletter:</p>
            <p style=""margin:24px 0"">
                <a href=""{m.ConfirmLink}""
                   style=""background:#2563eb;color:#fff;padding:12px 22px;border-radius:8px;text-decoration:none;display:inline-block"">
                    Confirm subscription
                </a>
            </p>
            <p style=""font-size:13px;color:#666"">
                If you didn't request this, you can safely ignore this email.
            </p>
            <p style=""font-size:13px;color:#666"">If the button doesn't work, paste this link into your browser:<br>{m.ConfirmLink}</p>
        </div>";
}
