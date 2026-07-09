using Application.Common.Interfaces.Repositories;
using Application.Features.Subscribers.DTOs;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.Subscribers.Commands.Unsubscribe;

public class UnsubscribeCommandHandler : IRequestHandler<UnsubscribeCommand, SubscriptionResponseDto>
{
    private readonly ISubscriberRepository _subscriberRepository;

    public UnsubscribeCommandHandler(ISubscriberRepository subscriberRepository)
    {
        _subscriberRepository = subscriberRepository;
    }

    public async Task<SubscriptionResponseDto> Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            throw new ValidationException("Unsubscribe token is required.");

        var subscriber = await _subscriberRepository.GetByUnsubscribeTokenAsync(request.Token)
            ?? throw new ValidationException("Invalid unsubscribe link.");

        if (subscriber.Status != SubscriptionStatus.Unsubscribed)
        {
            subscriber.Status = SubscriptionStatus.Unsubscribed;
            await _subscriberRepository.UpdateAsync(subscriber);
        }

        return new SubscriptionResponseDto("You've been unsubscribed. Sorry to see you go!");
    }
}
