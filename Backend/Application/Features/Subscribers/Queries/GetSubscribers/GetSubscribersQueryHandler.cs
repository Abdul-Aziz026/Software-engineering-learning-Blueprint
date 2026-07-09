using Application.Common.Interfaces.Repositories;
using Application.Common.Security;
using Application.Features.Subscribers.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.Subscribers.Queries.GetSubscribers;

public class GetSubscribersQueryHandler : IRequestHandler<GetSubscribersQuery, List<SubscriberDto>>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IUserRepository _userRepository;

    public GetSubscribersQueryHandler(ISubscriberRepository subscriberRepository, IUserRepository userRepository)
    {
        _subscriberRepository = subscriberRepository;
        _userRepository = userRepository;
    }

    public async Task<List<SubscriberDto>> Handle(GetSubscribersQuery request, CancellationToken cancellationToken)
    {
        await RoleGuard.EnsureAdminAsync(_userRepository, request.ActingUserId);

        var subscribers = await _subscriberRepository.GetAllAsync<Subscriber>();
        return subscribers
            .OrderByDescending(s => s.SubscribedAt)
            .Select(SubscriberDto.FromEntity)
            .ToList();
    }
}
