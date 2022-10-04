namespace BooksWishlist.Infrastructure.Store;

public class UserWishlistsRepository : IUserWishlistsRepository
{
    private readonly ILoggerService _log;
    private readonly BooksWishlistUnitOfWork<UserWishlists> _unitOfWork;

    public UserWishlistsRepository(ILoggerService log, IOptions<StoreDatabaseSettings> storeSettings)
    {
        _unitOfWork = new BooksWishlistUnitOfWork<UserWishlists>(storeSettings, log, "UserWishlists");
        _log = log;
    }

    public async Task<UserWishlists> CreateAsync(UserWishlists list, CancellationToken cancellationToken = default)
    {
        var listExists = await WishListExists(list.Name, list.OwnerId, cancellationToken);
        if (listExists)
        {
            throw new DuplicateEntityException($"The list with name: {list.Name} already exists in the database.");
        }

        await _unitOfWork.CreateAsync(list, cancellationToken);
        return list;
    }

    public async Task<bool> DeleteAsync(string listName, string owner, CancellationToken cancellationToken = default)
    {
        var filterDefinition = GetFilterByNameAndOwner(listName, owner);
        var foundList = await _unitOfWork.GetOneAsync(filterDefinition, cancellationToken);
        if (foundList is null)
        {
            throw new WishListNotFoundException();
        }

        await _unitOfWork.RemoveAsync(filterDefinition, cancellationToken);
        _log.LogWarning($"The Wishlist with name {listName} associated with the user {owner} and was deleted.");
        return true;
    }

    public Task<UserWishlists?> FindByNameAsync(string listName, string owner,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();


    public async Task<IEnumerable<UserWishlists?>> FindByOwnerAsync(string owner,
        CancellationToken cancellationToken = default)
    {
        var filterDefinition = new FilterDefinitionBuilder<UserWishlists>().Eq(l => l.OwnerId, owner);
        return await _unitOfWork.GetAsync(filterDefinition, cancellationToken);
    }

    private static FilterDefinition<UserWishlists> GetFilterByListName(string name) =>
        Builders<UserWishlists>.Filter.EqCase(list => list.Name, name, true);

    private static FilterDefinition<UserWishlists> GetFilterByNameAndOwner(string listName, string owner)
    {
        var filterByName = GetFilterByListName(listName);
        var filterDefinition = new FilterDefinitionBuilder<UserWishlists>().And(filterByName,
            new FilterDefinitionBuilder<UserWishlists>().Eq(l => l.OwnerId, owner));
        return filterDefinition;
    }


    private async Task<bool> WishListExists(string listName, string owner,
        CancellationToken cancellationToken = default)
    {
        var filterDefinition = GetFilterByNameAndOwner(listName, owner);
        var count = await _unitOfWork.CountAsync(filterDefinition, cancellationToken);
        return count > 0;
    }
}
