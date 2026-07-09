using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Security;
using Application.Features.Subscribers.DTOs;
using Application.Settings;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Subscribers.Commands.Subscribe;

public class SubscribeCommandHandler : IRequestHandler<SubscribeCommand, SubscriptionResponseDto>
{
    // Same response whether the address is new, pending, or already confirmed — no enumeration.
    private const string GenericResponse =
        "Almost there! Check your inbox for a link to confirm your subscription.";

    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IEmailSender _emailSender;
    private readonly PasswordResetOptions _options; // reused for its FrontendUrl (frontend base URL)
    private readonly ILogger<SubscribeCommandHandler> _logger;

    public SubscribeCommandHandler(
        ISubscriberRepository subscriberRepository,
        IEmailSender emailSender,
        PasswordResetOptions options,
        ILogger<SubscribeCommandHandler> logger)
    {
        _subscriberRepository = subscriberRepository;
        _emailSender = emailSender;
        _options = options;
        _logger = logger;
    }

    public async Task<SubscriptionResponseDto> Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        // Email is validated + normalized by SubscribeCommandValidator / the Email value object.
        var email = Email.Create(request.Email);

        var existing = await _subscriberRepository.GetByEmailAsync(email.Value);

        // Already confirmed: do nothing, but return the same message to avoid leaking membership.
        if (existing is { Status: SubscriptionStatus.Confirmed })
            return new SubscriptionResponseDto(GenericResponse);

        var rawToken = ResetTokenUtil.GenerateRawToken();
        var tokenHash = ResetTokenUtil.Hash(rawToken);

        if (existing is null)
        {
            var subscriber = new Subscriber
            {
                Email = email,
                Status = SubscriptionStatus.Pending,
                ConfirmationTokenHash = tokenHash,
                UnsubscribeToken = ResetTokenUtil.GenerateRawToken(),
                SubscribedAt = DateTime.UtcNow
            };
            await _subscriberRepository.AddAsync(subscriber);
        }
        else
        {
            // Pending or previously unsubscribed: reset the opt-in and re-send confirmation.
            existing.Status = SubscriptionStatus.Pending;
            existing.ConfirmationTokenHash = tokenHash;
            if (string.IsNullOrEmpty(existing.UnsubscribeToken))
                existing.UnsubscribeToken = ResetTokenUtil.GenerateRawToken();
            existing.ConfirmedAt = null;
            existing.SubscribedAt = DateTime.UtcNow;
            await _subscriberRepository.UpdateAsync(existing);
        }

        try
        {
            var confirmLink = $"{_options.FrontendUrl.TrimEnd('/')}/confirm-subscription?token={rawToken}";
            await _emailSender.SendAsync(
                email.Value,
                "Confirm your newsletter subscription",
                BuildEmailBody(confirmLink),
                cancellationToken);
        }
        catch (Exception ex)
        {
            // Don't surface delivery failures (would leak membership / internals); log for diagnostics.
            _logger.LogError(ex, "Failed to send subscription confirmation email.");
        }

        return new SubscriptionResponseDto(GenericResponse);
    }

    private static string BuildEmailBody(string confirmLink) => $@"
        <div style=""font-family:Arial,sans-serif;font-size:15px;color:#1a1a1a;line-height:1.6"">
            <p>Thanks for subscribing!</p>
            <p>Please confirm your email address to start receiving our newsletter:</p>
            <p style=""margin:24px 0"">
                <a href=""{confirmLink}""
                   style=""background:#2563eb;color:#fff;padding:12px 22px;border-radius:8px;text-decoration:none;display:inline-block"">
                    Confirm subscription
                </a>
            </p>
            <p style=""font-size:13px;color:#666"">
                If you didn't request this, you can safely ignore this email.
            </p>
            <p style=""font-size:13px;color:#666"">If the button doesn't work, paste this link into your browser:<br>{confirmLink}</p>
        </div>";
}
