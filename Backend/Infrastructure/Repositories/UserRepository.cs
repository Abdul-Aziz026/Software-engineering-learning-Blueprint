
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class UserRepository : Repository, IUserRepository
{
    public UserRepository(IDatabaseContext dbContext) : base(dbContext)
    {
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return DbContext.GetItemByConditionAsync<User>(u => u.Email == normalized);
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        var normalized = username.Trim().ToLowerInvariant();
        return DbContext.GetItemByConditionAsync<User>(u => u.Username == normalized);
    }

    public Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername)
    {
        var normalized = emailOrUsername.Trim().ToLowerInvariant();
        return DbContext.GetItemByConditionAsync<User>(
            u => u.Email == normalized || u.Username == normalized);
    }
}
