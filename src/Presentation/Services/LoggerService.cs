namespace BooksWishlist.Presentation.Services;

public class LoggerService : ILoggerService
{
    public void Log(string message)
    {
        WatchLogger.Log(message);
    }
}
