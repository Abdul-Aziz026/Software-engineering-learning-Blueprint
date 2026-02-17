
using Application.Common.Interfaces.Persistence;
using Application.Settings;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Persistence;

public class DatabaseContext : IDatabaseContext
{
    private readonly MongoSettings _mongoSettings;
    private readonly ILogger<DatabaseContext> _logger;
    private MongoClient _client;
    private IClientSessionHandle _session;
    public DatabaseContext(ILogger<DatabaseContext> logger,
                           IOptions<MongoSettings> setting)
    {
        _logger = logger;
        _mongoSettings = setting.Value;
    }
    private MongoClient GetClient()
    {
        if (_client == null)
        {
            // network compression options:
            // reduces the amount of data passed over the network between mongodb & app.
            // var uri = $"{options.Value.ConnectionString}?compressors=snappy,zlib,zstd";
            // var settings = MongoClientSettings.FromConnectionString(uri);

            var settings = MongoClientSettings.FromConnectionString(_mongoSettings.ConnectionString);
            //settings.UseTls = _mongoSettings.UseTls;
            settings.MaxConnecting = _mongoSettings.MaxConnecting;
            settings.MinConnectionPoolSize = _mongoSettings.MinConnectionPoolSize;
            settings.MaxConnectionPoolSize = _mongoSettings.MaxConnectionPoolSize;
            settings.MaxConnectionLifeTime = TimeSpan.FromMinutes(_mongoSettings.MaxConnectionLifeTime);
            settings.WaitQueueTimeout = TimeSpan.FromSeconds(_mongoSettings.WaitQueTimeout);
            settings.RetryWrites = _mongoSettings.RetryWrites;
            settings.RetryWrites = _mongoSettings.RetryReads;
            settings.WriteConcern = WriteConcern.WMajority;
            settings.ReadConcern = ReadConcern.Majority;

            _client = new MongoClient(settings);
        }
        return _client;
    }
    private IMongoDatabase GetDatabase(string indexInfo)
    {
        return GetClient().GetDatabase(indexInfo);
    }
    public IMongoCollection<T> GetCollection<T>(string? name = null!)
    {
        name = name ?? typeof(T).Name.ToLower();
        return GetDatabase(_mongoSettings.DatabaseName).GetCollection<T>(name.ToLower());
    }

    public async Task<List<T>> GetAllAsync<T>() where T : class
    {
        try
        {
            var collection = GetCollection<T>();
            var response = await collection.Find(_ => true).ToListAsync();
            _logger.LogInformation($"Retrieved all entities of type {typeof(T).FullName}, count: {response.Count}");
            return response;
        }
        catch
        {
            _logger.LogError($"GetAllAsync failed for type {typeof(T).FullName}");
            return default!;
        }
    }

    public async Task<bool> AddAsync<T>(T entity) where T : BaseEntity
    {
        try
        {
            var collection = GetCollection<T>();
            if (_session != null)
            {
                await collection.InsertOneAsync(_session, entity);
            }
            else
            {
                await collection.InsertOneAsync(entity);
            }
            _logger.LogInformation($"Added entity of type {typeof(T).FullName} with Id {entity.Id}");
            return true;
        }
        catch
        {
            _logger.LogError($"AddAsync failed for type {typeof(T).FullName} with Id {entity.Id}");
            return false;
        }
    }

    public async Task<bool> UpdateAsync<T>(T entity) where T : BaseEntity
    {
        try
        {
            var collection = GetCollection<T>();
            var result = await collection.ReplaceOneAsync(o => o.Id == entity.Id, entity);
            var success = result.IsAcknowledged && result.ModifiedCount > 0;
            if (success)
            {
                _logger.LogInformation($"Updated entity of type {typeof(T).FullName} with Id {entity.Id}");
            }
            else
            {
                _logger.LogWarning($"UpdateAsync did not modify any document for type {typeof(T).FullName} with Id {entity.Id}");
            }
            return success;
        }
        catch (Exception ex)  // 💡 Also capture the exception
        {
            _logger.LogError(ex, $"UpdateAsync failed for type {typeof(T).FullName} with Id {entity.Id}");
            return false;
        }
    }

    public async Task<bool> DeleteAsync<T>(T entity) where T : BaseEntity
    {
        try
        {
            var collection = GetCollection<T>();
            await collection.DeleteOneAsync(o => o.Id.Equals(entity.Id));
            _logger.LogInformation($"Deleted entity of type {typeof(T).FullName} with Id {entity.Id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"DeleteAsync failed for type {typeof(T).FullName} with Id {entity.Id}");
            return false;
        }
    }

    public async Task<int> DeleteManyAsync<T>(IEnumerable<T> entities) where T : BaseEntity
    {
        try
        {
            var collection = GetCollection<T>();
            var ids = entities.Select(e => e.Id);
            var result = await collection.DeleteManyAsync(
                Builders<T>.Filter.In(x => x.Id, ids)
            );

            _logger.LogInformation(
                $"Deleted {result.DeletedCount} {typeof(T).Name} documents");

            return (int)result.DeletedCount;
        }
        catch
        {
            _logger.LogError($"DeleteManyAsync failed for type {typeof(T).FullName}");
            return 0;
        }
    }

    public async Task<T?> GetItemByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity
    {
        var collection = GetCollection<T>();
        var filter = Builders<T>.Filter.Where(criteria);
        var response = await collection.Find(filter).FirstOrDefaultAsync();
        return response;
    }

    public async Task<List<T>?> GetItemsByConditionAsync<T>(Expression<Func<T, bool>> criteria) where T : BaseEntity
    {
        var collection = GetCollection<T>();
        var filter = Builders<T>.Filter.Where(criteria);
        var results = await collection
            .Find(filter)
            .ToListAsync();   // fetch all matching documents
        return results;
    }

    public async Task<long> CountAsync<T>(Expression<Func<T, bool>> criteria) where T : class
    {
        var collection = GetCollection<T>();
        var filter = Builders<T>.Filter.Where(criteria);
        // optional
        var options = new CountOptions
        {
            Hint = "_id_"
        };
        return await collection.CountDocumentsAsync(filter, options);
    }

    public async Task<List<T>> GetPagedResponseAsync<T>(Expression<Func<T, bool>>? criteria = null,
                                                 int pageNumber = 1,
                                                 int pageSize = 10,
                                                 Expression<Func<T, object>>? orderBy = null,
                                                 bool ascending = true)
    {
        var collection = GetCollection<T>();
        var filter = criteria != null ? Builders<T>.Filter.Where(criteria) : Builders<T>.Filter.Empty;

        var query = collection.Find(filter);

        if (orderBy is not null)
        {
            var sortDefinition = ascending
                ? Builders<T>.Sort.Ascending(orderBy)
                : Builders<T>.Sort.Descending(orderBy);

            query = query.Sort(sortDefinition);
        }
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return items;
    }

    public async Task<bool> DeleteByIdAsync<T>(string id) where T : BaseEntity
    {
        try
        {
            var collection = GetCollection<T>();
            await collection.DeleteOneAsync(o => o.Id.Equals(id));
            _logger.LogInformation($"Deleted entity of type {typeof(T).FullName} with Id {id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"DeleteAsync failed for type {typeof(T).FullName} with Id {id}");
            return false;
        }
    }


    public async Task<(List<T>, string)> GetCursorPagedResponseAsync<T>(
        string? lastId = null,
        int pageSize = 10,
        Expression<Func<T, bool>>? criteria = null,
        bool ascending = true,
        CancellationToken cancellationToken = default) where T : BaseEntity
    {
        try
        {
            var collection = GetCollection<T>();

            // Build the filter
            var filterBuilder = Builders<T>.Filter;
            var filters = new List<FilterDefinition<T>>();

            // Add user criteria if provided
            if (criteria != null)
            {
                filters.Add(filterBuilder.Where(criteria));
            }

            // Add cursor filter based on lastId
            if (!string.IsNullOrEmpty(lastId))
            {
                var idFilter = ascending
                    ? filterBuilder.Gt(x => x.Id, lastId)
                    : filterBuilder.Lt(x => x.Id, lastId);
                filters.Add(idFilter);
            }

            // Combine filters
            var finalFilter = filters.Count > 0
                ? filterBuilder.And(filters)
                : filterBuilder.Empty;

            // Build sort definition - always sort by Id for cursor-based pagination
            var sortDefinition = ascending
                ? Builders<T>.Sort.Ascending(x => x.Id)
                : Builders<T>.Sort.Descending(x => x.Id);

            // Execute query - fetch pageSize + 1 to determine if there are more items
            var items = await collection
                .Find(finalFilter)
                .Sort(sortDefinition)
                .Limit(pageSize + 1)
                .ToListAsync();

            // Determine next cursor
            string nextCursor = null;
            bool hasMore = items.Count > pageSize;

            if (hasMore)
            {
                // Remove the extra item
                items.RemoveAt(pageSize);

                // Get the Id of the last item as the next cursor
                nextCursor = items[^1].Id;
            }

            _logger.LogInformation(
                $"Retrieved cursor-based page for type {typeof(T).FullName}, " +
                $"count: {items.Count}, hasMore: {hasMore}, nextCursor: {nextCursor}");

            return (items, nextCursor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"GetCursorPagedResponseAsync failed for type {typeof(T).FullName}");
            return (new List<T>(), null);
        }
    }

    /// <summary>
    /// /// <summary>
    /// Begin a transaction
    /// </summary>
    public IDatabaseContext BeginTransaction()
    {
        try
        {
            if (_session != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }
            _session = GetClient().StartSession();
            _session.StartTransaction();

            _logger.LogInformation("Transaction started.");
            return this;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "BeginTransaction failed");
            throw;
        }
    }

    /// <summary>
    /// Commit transaction
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        try
        {
            if (_session == null)
            {
                throw new InvalidOperationException("No active transaction to commit.");
            }
            await _session.CommitTransactionAsync();
            _logger.LogInformation("Transaction committed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CommitTransactionAsync failed");
            throw;
        }
        finally
        {
            _session?.Dispose();
            _session = null;
        }
    }
    /// <summary>
    /// Rollback transaction
    /// </summary>
    public async Task AbortTransactionAsync()
    {
        try
        {
            if (_session == null)
            {
                throw new InvalidOperationException("No active transaction to abort.");
            }
            await _session.AbortTransactionAsync();
            _logger.LogInformation("Transaction aborted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AbortTransactionAsync failed");
            throw;
        }
        finally
        {
            _session?.Dispose();
            _session = null;
        }
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    public void Dispose()
    {
        if (_session != null)
        {
            _session.CommitTransaction();
            _session.Dispose();
            _session = null;
        }
    }
    /*
// With criteria
var (filtered, cursor) = await database.GetCursorPagedResponseAsync<Product, object>(
    lastId: null,
    pageSize: 20,
    criteria: p => p.Status == true,
    cancellationToken: cancellationToken
);
     */
}
