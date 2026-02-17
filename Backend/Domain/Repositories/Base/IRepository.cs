using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Repositories.Base;

public interface IRepository
{
    Task<bool> AddAsync<T>(T entity) where T : BaseEntity;
    Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity;
    Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity;
    Task<bool> DeleteByIdAsync<T>(string Id) where T : BaseEntity;
    //Task<int> CountAsync(ISpecification<T> spec);
    Task<List<T>> GetAllAsync<T>() where T : class;
    Task<T?> GetByIdAsync<T>(string Id) where T: BaseEntity;
    Task<T?> GetItemByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity;
    Task<List<T>?> GetItemsByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity;
    Task<long> CountAsync<T>(Expression<Func<T, bool>> filter) where T : class;
    Task<List<T>> GetPagedAsync<T>(Expression<Func<T, bool>>? filter = null,
        int pageNumber = 1,
        int pageSize = 10,
        Expression<Func<T, object>>? orderBy = null,
        bool ascending = true);
}
