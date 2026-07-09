using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Indexing;

/// <summary>
/// Unique index on email (one subscriber per address; also serves GetByEmailAsync),
/// plus a lookup index on the confirmation/unsubscribe token hash.
/// </summary>
public sealed class SubscriberIndexes : MongoIndexConfiguration<Subscriber>
{
    protected override IEnumerable<CreateIndexModel<Subscriber>> BuildIndexes()
    {
        yield return new CreateIndexModel<Subscriber>(
            Builders<Subscriber>.IndexKeys.Ascending(s => s.Email),
            new CreateIndexOptions { Name = "subscriber_email_unique", Unique = true });

        yield return new CreateIndexModel<Subscriber>(
            Builders<Subscriber>.IndexKeys.Ascending(s => s.ConfirmationTokenHash),
            new CreateIndexOptions { Name = "subscriber_tokenhash" });

        yield return new CreateIndexModel<Subscriber>(
            Builders<Subscriber>.IndexKeys.Ascending(s => s.UnsubscribeToken),
            new CreateIndexOptions { Name = "subscriber_unsubscribe_token" });
    }
}
