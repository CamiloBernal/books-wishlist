using System.Linq.Expressions;
using BooksWishlist.Infrastructure.Services;
using BooksWishlist.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace BooksWishlist.Infrastructure.Databases;

public class BooksWishlistRepository<T> where T : class, new()
{
    private readonly IMongoCollection<T> _collection;
    private readonly ILoggerService _logger;

    public BooksWishlistRepository(IOptions<StoreDatabaseSettings> storeSettings, ILoggerService logger,
        string collectionName)
    {
        _logger = logger;
        try
        {
            var mongoClient = new MongoClient(storeSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(storeSettings.Value.DatabaseName);
            _collection = mongoDatabase.GetCollection<T>(collectionName);
            Configure();
        }
        catch (Exception e)
        {
            _logger.LogError("Error configuring Database", e);
            throw;
        }
    }

    private static void Configure()
    {
        var pack = new ConventionPack { new GuidObjectIdConvention() };
        ConventionRegistry.Register("GuidObjectIdConvention", pack, _ => true);
    }


    public async Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error with a query in the {_collection} collection.", e);
            throw;
        }
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error with a query in the {_collection} collection.", e);
            throw;
        }
    }

    public async Task CreateAsync(T newEntity, CancellationToken cancellationToken = default)
    {
        try
        {
            await _collection.InsertOneAsync(newEntity, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error on creating item in a {_collection} collection.", e);
            throw;
        }
    }

    public async Task UpdateAsync(FilterDefinition<T> filter, T updatedEntity,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _collection.ReplaceOneAsync(filter, updatedEntity, cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error on updating item in a {_collection} collection.", e);
            throw;
        }
    }

    public async Task RemoveAsync(FilterDefinition<T> filter, CancellationToken cancellationToken = default)
    {
        try
        {
            await _collection.DeleteOneAsync(filter, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error deleting item in a {_collection} collection.", e);
            throw;
        }
    }
}
