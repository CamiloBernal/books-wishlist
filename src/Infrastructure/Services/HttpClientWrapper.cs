using System.Net.Http.Json;

namespace BooksWishlist.Infrastructure.Services;

public class HttpClientWrapper<T> where T : class, new()
{
    private readonly ILoggerService _log;

    public HttpClientWrapper(ILoggerService log) => _log = log;

    public async Task<T?> GetAsync(string resourceQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Constants.GoogleBooksServiceUrlBase);
            var result = await httpClient.GetFromJsonAsync<T>(resourceQuery, cancellationToken);
            return result;
        }
        catch (HttpRequestException httpRequestException)
        {
            _log.LogError(httpRequestException.Message, httpRequestException);
            if (httpRequestException.Message.ToLower().Contains("bad request"))
            {
                throw new GoogleServiceBadRequestException();
            }

            throw;
        }
        catch (Exception e)
        {
            _log.LogError(e.Message, e);
            throw;
        }
    }
}
