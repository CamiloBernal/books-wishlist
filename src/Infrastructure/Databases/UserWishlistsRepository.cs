using BooksWishlist.Application.UserWishlists.Entities;
using BooksWishlist.Infrastructure.Extensions;
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
}
