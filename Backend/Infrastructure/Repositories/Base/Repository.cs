using Application.Common.Interfaces.Persistence;
using Domain.Entities;
using Domain.Repositories.Base;
using Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Base;

public class Repository : IRepository
{
    protected readonly IDatabaseContext DbContext;
    public Repository(IDatabaseContext dbContext)
    {
        DbContext = dbContext;
    }
    public async Task<bool> AddAsync<T>(T entity) where T : BaseEntity
    {
        return await DbContext.AddAsync<T>(entity);
    }

    public async Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity
    {
        return await DbContext.DeleteAsync<T>(entity);
    }

    public async Task<bool> DeleteByIdAsync<T>(string Id) where T : BaseEntity
    {
        return await DbContext.DeleteByIdAsync<T>(Id);
    }

    public async Task<List<T>> GetAllAsync<T>() where T : class
    {
        return await DbContext.GetAllAsync<T>();
    }

    public async Task<T?> GetByIdAsync<T>(string userId) where T : BaseEntity
    {
        return await DbContext.GetItemByConditionAsync<T>(u => u.Id == userId);
    }

    public async Task<T?> GetItemByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity
    {
        return await DbContext.GetItemByConditionAsync<T>(criteria);
    }

    public async Task<List<T>?> GetItemsByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity
    {
        return await DbContext.GetItemsByConditionAsync<T>(criteria);
    }

    public async Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity
    {
        return await DbContext.UpdateAsync<T>(entity);
    }
    
    public async Task<List<T>> GetPagedAsync<T>(Expression<Func<T, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 10,
        Expression<Func<T, object>>? orderBy = null,
        bool ascending = true)
    {
        return await DbContext.GetPagedResponseAsync<T>(filter, pageNumber, pageSize, orderBy, ascending);
    }

    public async Task<long> CountAsync<T>(Expression<Func<T, bool>> filter) where T : class
    {
        return await DbContext.CountAsync<T>(filter);
    }
}
