using Application.Features.Subscribers.DTOs;
using MediatR;

namespace Application.Features.Subscribers.Commands.Unsubscribe;

public class UnsubscribeCommand : IRequest<SubscriptionResponseDto>
{
    // Raw token from the unsubscribe link in a newsletter email.
    public string Token { get; set; } = string.Empty;
}
