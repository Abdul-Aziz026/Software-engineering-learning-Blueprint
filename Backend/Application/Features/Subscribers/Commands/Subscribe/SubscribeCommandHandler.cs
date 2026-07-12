using Application.Common.Interfaces.Publisher;
using Application.Common.Interfaces.Repositories;
using Application.Common.Security;
using Application.Features.Subscribers.DTOs;
using Application.Settings;
using Contracts.Messaging;
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
    private readonly IMessageBus _messageBus;
    private readonly PasswordResetOptions _options; // reused for its FrontendUrl (frontend base URL)
    private readonly ILogger<SubscribeCommandHandler> _logger;

    public SubscribeCommandHandler(
        ISubscriberRepository subscriberRepository,
        IMessageBus messageBus,
        PasswordResetOptions options,
        ILogger<SubscribeCommandHandler> logger)
    {
        _subscriberRepository = subscriberRepository;
        _messageBus = messageBus;
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
            // Off-thread delivery via the messaging pipeline. Wrapped so a broker hiccup can't leak
            // membership / internals to the caller.
            var confirmLink = $"{_options.FrontendUrl.TrimEnd('/')}/confirm-subscription?token={rawToken}";
            await _messageBus.PublishAsync(new SendSubscriptionConfirmation(email.Value, confirmLink));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue subscription confirmation email.");
        }

        return new SubscriptionResponseDto(GenericResponse);
    }
}
