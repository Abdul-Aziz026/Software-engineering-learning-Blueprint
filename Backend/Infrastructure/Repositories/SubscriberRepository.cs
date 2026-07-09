using Application.Common.Events;
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class SubscriberRepository : Repository, ISubscriberRepository
{
    public SubscriberRepository(IDatabaseContext dbContext, IDomainEventDispatcher domainEventDispatcher)
        : base(dbContext, domainEventDispatcher)
    {
    }

    public Task<Subscriber?> GetByEmailAsync(string email)
    {
        return Email.TryCreate(email, out var parsed)
            ? DbContext.GetItemByConditionAsync<Subscriber>(s => s.Email == parsed)
            : Task.FromResult<Subscriber?>(null);
    }

    public Task<Subscriber?> GetByTokenHashAsync(string tokenHash)
    {
        return DbContext.GetItemByConditionAsync<Subscriber>(s => s.ConfirmationTokenHash == tokenHash);
    }

    public Task<Subscriber?> GetByUnsubscribeTokenAsync(string unsubscribeToken)
    {
        return DbContext.GetItemByConditionAsync<Subscriber>(s => s.UnsubscribeToken == unsubscribeToken);
    }

    public async Task<List<Subscriber>> GetConfirmedAsync()
    {
        var confirmed = await DbContext.GetItemsByConditionAsync<Subscriber>(
            s => s.Status == SubscriptionStatus.Confirmed);
        return confirmed ?? new List<Subscriber>();
    }
}
