namespace BooksWishlist.Infrastructure.Services;

public class SecurityService : ISecurityService
{
    private readonly ILoggerService _log;

    public SecurityService(ILoggerService log)
    {
        _log = log;
    }

    public Task<bool> RegisterUserAsync(User user)
    {
        _log.Log("User registration requested.");
        throw new NotImplementedException();
    }
}
