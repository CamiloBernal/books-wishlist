namespace BooksWishlist.Infrastructure.Store;

public class BooksWishlistUnitOfWork<T> where T : class, new()
{
    private readonly IMongoCollection<T> _collection;
    private readonly ILoggerService _logger;

    public BooksWishlistUnitOfWork(IOptions<StoreDatabaseSettings> storeSettings, ILoggerService logger,
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
        var pack = new ConventionPack { new StringObjectIdConvention() };
        ConventionRegistry.Register("GuidObjectIdConvention", pack, _ => true);
    }


    public async Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error with a query in the {_collection} collection.", e);
            throw;
        }
    }

    public async Task<IEnumerable<T?>> GetAsync(FilterDefinition<T> filterDefinition,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _collection.Find(filterDefinition).ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error with a query in the {_collection} collection.", e);
            throw;
        }
    }

    public async Task<T?> GetOneAsync(FilterDefinition<T> filterDefinition,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _collection.Find(filterDefinition).FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error with a query in the {_collection} collection.", e);
            throw;
        }
    }

    public async Task<long> CountAsync(FilterDefinition<T> filterDefinition,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _collection.CountDocumentsAsync(filterDefinition, cancellationToken: cancellationToken);
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
