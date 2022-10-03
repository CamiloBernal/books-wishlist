using BooksWishlist.Application.UserWishlists.Entities;
using BooksWishlist.Infrastructure.Services;

namespace BooksWishlist.Infrastructure.Databases;

public class UserWishlistsRepository : IUserWishlistsRepository
{
    private readonly BooksWishlistUnitOfWork<UserWishlists> _unitOfWork;
    private ILoggerService _log;

    public UserWishlistsRepository(ILoggerService log, IOptions<StoreDatabaseSettings> storeSettings)
    {
        _unitOfWork = new BooksWishlistUnitOfWork<UserWishlists>(storeSettings, log, "UserWishlists");
        _log = log;
    }

    public async Task<UserWishlists> Create(UserWishlists list, CancellationToken cancellationToken = default)
    {
        var listExists = await WishListExists(list.Name, list.OwnerId, cancellationToken);
        if (listExists)
        {
            throw new DuplicateEntityException($"The list with name: {list.Name} already exists in the database.");
        }

        await _unitOfWork.CreateAsync(list, cancellationToken);
        return list;
    }

    public async Task<UserWishlists?> FindByName(string name, CancellationToken cancellationToken = default)
    {
        var filterDefinition = GetFilterByListName(name);
        return await _unitOfWork.GetAsync(filterDefinition, cancellationToken);
    }

    private static FilterDefinition<UserWishlists> GetFilterByListName(string name) =>
        Builders<UserWishlists>.Filter.EqCase(list => list.Name, name, true);


    private async Task<bool> WishListExists(string listName, string owner,
        CancellationToken cancellationToken = default)
    {
        var filterByName = GetFilterByListName(listName);
        var filterDefinition = new FilterDefinitionBuilder<UserWishlists>().And(new[]
            {
                filterByName, new FilterDefinitionBuilder<UserWishlists>().Eq(l => l.OwnerId, owner)
            }
        );
        var count = await _unitOfWork.CountAsync(filterDefinition, cancellationToken);
        return count > 0;
    }
}
