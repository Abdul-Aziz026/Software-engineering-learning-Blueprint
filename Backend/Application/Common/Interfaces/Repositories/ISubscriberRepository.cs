using Domain.Entities;
using Domain.Repositories.Base;

namespace Application.Common.Interfaces.Repositories;

public interface ISubscriberRepository : IRepository
{
    Task<Subscriber?> GetByEmailAsync(string email);
    Task<Subscriber?> GetByTokenHashAsync(string tokenHash);
    Task<Subscriber?> GetByUnsubscribeTokenAsync(string unsubscribeToken);

    // All confirmed subscribers — the newsletter fan-out audience.
    Task<List<Subscriber>> GetConfirmedAsync();
}
