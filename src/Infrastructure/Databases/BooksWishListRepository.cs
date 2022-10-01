using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BooksWishlist.Infrastructure.Databases;

public class BooksWishlistRepository<T> where T : class, new()
{
    private readonly IMongoCollection<T> _collection;

    public BooksWishlistRepository(IOptions<StoreDatabaseSettings> storeOptions, string collectionName)
    {
        var mongoClient = new MongoClient(storeOptions.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(storeOptions.Value.DatabaseName);
        _collection = mongoDatabase.GetCollection<T>(collectionName);
    }

    public async Task<IEnumerable<T>> GetAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T newEntity)
    {
        await _collection.InsertOneAsync(newEntity);
    }

    public async Task UpdateAsync(FilterDefinition<T> filter, T updatedEntity)
    {
        await _collection.ReplaceOneAsync(filter, updatedEntity);
    }

    public async Task RemoveAsync(FilterDefinition<T> filter)
    {
        await _collection.DeleteOneAsync(filter);
    }
}
