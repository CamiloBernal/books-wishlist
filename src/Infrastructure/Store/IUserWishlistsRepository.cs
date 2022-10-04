using BooksWishlist.Application.Books.Entities;

namespace BooksWishlist.Infrastructure.Store;

public interface IUserWishlistsRepository
{
    Task<UserWishlists> CreateAsync(UserWishlists list, CancellationToken cancellationToken = default);

    Task<UserWishlists> UpdateAsync(UserWishlists list, string listName, string owner,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<UserWishlists?>> FindByOwnerAsync(string owner, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string listName, string owner, CancellationToken cancellationToken = default);

    Task<bool?> AddBooksAsync(string listName, IEnumerable<Book?>? books, string owner,
        CancellationToken cancellationToken = default);

    Task<UserWishlists?> FindByNameAsync(string listName, string owner, CancellationToken cancellationToken = default);
}
