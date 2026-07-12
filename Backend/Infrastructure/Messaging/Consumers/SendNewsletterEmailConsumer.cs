using Application.Common.Interfaces.Services;
using Contracts.Messaging;
using MassTransit;

namespace Infrastructure.Messaging.Consumers;

/// <summary>
/// Delivers one newsletter email. Lets exceptions escape so MassTransit's retry policy engages and,
/// once exhausted, dead-letters the message to the "&lt;queue&gt;_error" queue.
/// </summary>
public class SendNewsletterEmailConsumer : IConsumer<SendNewsletterEmail>
{
    private readonly IEmailSender _emailSender;

    public SendNewsletterEmailConsumer(IEmailSender emailSender) => _emailSender = emailSender;

    public Task Consume(ConsumeContext<SendNewsletterEmail> context)
    {
        var m = context.Message;
        return _emailSender.SendAsync(m.ToEmail, m.Title, BuildEmailBody(m), context.CancellationToken);
    }

    private static string BuildEmailBody(SendNewsletterEmail m) => $@"
        <div style=""font-family:Arial,sans-serif;font-size:15px;color:#1a1a1a;line-height:1.6"">
            <h2 style=""margin:0 0 12px"">{System.Net.WebUtility.HtmlEncode(m.Title)}</h2>
            <p>{System.Net.WebUtility.HtmlEncode(m.Summary)}</p>
            <p style=""margin:24px 0"">
                <a href=""{m.PostLink}""
                   style=""background:#2563eb;color:#fff;padding:12px 22px;border-radius:8px;text-decoration:none;display:inline-block"">
                    Read the full post
                </a>
            </p>
            <hr style=""border:none;border-top:1px solid #eee;margin:24px 0"">
            <p style=""font-size:12px;color:#999"">
                You're receiving this because you subscribed to our newsletter.
                <a href=""{m.UnsubscribeLink}"" style=""color:#999"">Unsubscribe</a>.
            </p>
        </div>";
}
