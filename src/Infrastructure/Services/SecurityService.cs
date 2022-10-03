using BooksWishlist.Infrastructure.Store;

namespace BooksWishlist.Infrastructure.Services;

public class SecurityService : ISecurityService
{
    private readonly CryptoService _crypto;
    private readonly ILoggerService _log;
    private readonly BooksWishlistUnitOfWork<User> _unitOfWork;

    public SecurityService(ILoggerService log, IOptions<StoreDatabaseSettings> storeSettings,
        IOptions<CryptoServiceSettings> cryptoSettings)
    {
        _log = log;
        _unitOfWork = new BooksWishlistUnitOfWork<User>(storeSettings, log, "Users");
        _crypto = new CryptoService(cryptoSettings);
    }

    public async Task<bool> RegisterUserAsync(User user, CancellationToken cancellationToken = default)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var userExists = await UserExists(user.UserName, cancellationToken);
        if (userExists)
        {
            throw new DuplicateEntityException($"The {user.UserName} user already exists in the database.");
        }

        user.Password = _crypto.EncryptString(user.Password);
        _log.LogInformation("User registration requested");
        await _unitOfWork.CreateAsync(user, cancellationToken);
        _log.LogInformation($"A new user has been registered in the database: {user.UserName}");
        return true;
    }

    public async Task<User?> ValidateUser(User user, CancellationToken cancellationToken = default)
    {
        var filterDefinition = GetFilterByUserName(user.UserName);
        var foundUser = await _unitOfWork.GetAsync(filterDefinition, cancellationToken);
        if (foundUser is null)
        {
            return null;
        }

        var encPass = _crypto.EncryptString(user.Password);
        return foundUser.Password.Equals(encPass) ? foundUser : null;
    }

    private static FilterDefinition<User> GetFilterByUserName(string userName) =>
        Builders<User>.Filter.EqCase(user => user.UserName, userName, true);


    private async Task<bool> UserExists(string userName, CancellationToken cancellationToken = default)
    {
        var filterDefinition = GetFilterByUserName(userName);
        var count = await _unitOfWork.CountAsync(filterDefinition, cancellationToken);
        return count > 0;
    }
}
