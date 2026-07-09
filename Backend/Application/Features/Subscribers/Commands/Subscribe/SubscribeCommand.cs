using Application.Features.Subscribers.DTOs;
using MediatR;

namespace Application.Features.Subscribers.Commands.Subscribe;

public class SubscribeCommand : IRequest<SubscriptionResponseDto>
{
    public string Email { get; set; } = string.Empty;
}
