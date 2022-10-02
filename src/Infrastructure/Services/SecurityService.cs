using BooksWishlist.Application.Exceptions;
using BooksWishlist.Infrastructure.Databases;
using BooksWishlist.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

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
        if (user == null) throw new ArgumentNullException(nameof(user));
        var userExists = await UserExists(user.UserName, cancellationToken);
        if (userExists) throw new DuplicateUserException($"The {user.UserName} user already exists in the database.");
        user.Password = _crypto.EncryptString(user.Password);
        _log.LogInformation("User registration requested");
        await _repository.CreateAsync(user, cancellationToken);
        _log.LogInformation($"A new user has been registered in the database: {user.UserName}");
        return true;
    }

    private async Task<bool> UserExists(string userName, CancellationToken cancellationToken = default)
    {
        var filterDefinition = Builders<User>.Filter.Where(u => u.UserName.Equals(userName));
        var count = await _repository.CountAsync(filterDefinition, cancellationToken);
        return count > 0;
    }

}
