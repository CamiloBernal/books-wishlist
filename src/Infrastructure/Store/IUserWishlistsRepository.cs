namespace BooksWishlist.Infrastructure.Store;

public interface IUserWishlistsRepository
{
    Task<UserWishlists> Create(UserWishlists list, CancellationToken cancellationToken = default);
    Task<UserWishlists?> FindByName(string name, CancellationToken cancellationToken = default);
}
