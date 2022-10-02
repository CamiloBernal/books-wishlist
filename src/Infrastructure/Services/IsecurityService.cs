namespace BooksWishlist.Infrastructure.Services;

public interface ISecurityService
{
    Task<bool> RegisterUserAsync(User user, CancellationToken cancellationToken = default);

    Task<User?> ValidateUser(User user, CancellationToken cancellationToken = default);
}
