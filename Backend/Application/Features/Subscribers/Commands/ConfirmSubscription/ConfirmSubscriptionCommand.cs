using Application.Features.Subscribers.DTOs;
using MediatR;

namespace Application.Features.Subscribers.Commands.ConfirmSubscription;

public class ConfirmSubscriptionCommand : IRequest<SubscriptionResponseDto>
{
    // Raw token from the confirmation link.
    public string Token { get; set; } = string.Empty;
}
