using BooksWishlist.Infrastructure.Databases;
using BooksWishlist.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BooksWishlist.Infrastructure.Services;

public class SecurityService : ISecurityService
{
    private readonly CryptoService _crypto;
    private readonly ILoggerService _log;
    private readonly BooksWishlistRepository<User> _repository;

    public SecurityService(ILoggerService log, IOptions<StoreDatabaseSettings> storeSettings,
        IOptions<CryptoServiceSettings> cryptoSettings)
    {
        _log = log;
        _repository = new BooksWishlistRepository<User>(storeSettings, log, "Users");
        _crypto = new CryptoService(cryptoSettings);
    }

    public async Task<bool> RegisterUserAsync(User user, CancellationToken cancellationToken = default )
    {
        user.Password = _crypto.EncryptString(user.Password);
        _log.LogInformation("User registration requested");
        await _repository.CreateAsync(user, cancellationToken);
        _log.LogInformation("A new user has been registered in the database: {UserName}", user.UserName);
        return true;
    }
}
