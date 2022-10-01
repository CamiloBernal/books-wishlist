namespace BooksWishlist.Infrastructure.Services;

public interface ISecurityService
{
    Task<bool> RegisterUserAsync(User user, CancellationToken cancellationToken = default);
}
