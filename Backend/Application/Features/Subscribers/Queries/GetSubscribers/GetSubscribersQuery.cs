using Application.Features.Subscribers.DTOs;
using MediatR;

namespace Application.Features.Subscribers.Queries.GetSubscribers;

public class GetSubscribersQuery : IRequest<List<SubscriberDto>>
{
    // The requesting user (from X-User-Id). Must be Admin or SuperAdmin.
    public string ActingUserId { get; set; } = string.Empty;
}
