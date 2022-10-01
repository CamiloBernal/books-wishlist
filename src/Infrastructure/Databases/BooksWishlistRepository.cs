using System.Linq.Expressions;
using BooksWishlist.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BooksWishlist.Infrastructure.Databases;

public class BooksWishlistRepository<T> where T : class, new()
{
    private readonly IMongoCollection<T> _collection;

    public BooksWishlistRepository(IOptions<StoreDatabaseSettings> storeSettings, string collectionName)
    {
        var mongoClient = new MongoClient(storeSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(storeSettings.Value.DatabaseName);
        _collection = mongoDatabase.GetCollection<T>(collectionName);
    }

    public async Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken  = default)
    {
        return await _collection.Find(_ => true).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken  = default)
    {
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public async Task CreateAsync(T newEntity, CancellationToken cancellationToken  = default)
    {
        await _collection.InsertOneAsync(newEntity, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(FilterDefinition<T> filter, T updatedEntity, CancellationToken cancellationToken  = default)
    {
        await _collection.ReplaceOneAsync(filter, updatedEntity, cancellationToken: cancellationToken);
    }

    public async Task RemoveAsync(FilterDefinition<T> filter, CancellationToken cancellationToken  = default)
    {
        await _collection.DeleteOneAsync(filter, cancellationToken);
    }
}
