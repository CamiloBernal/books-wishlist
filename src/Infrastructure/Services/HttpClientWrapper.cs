using System.Net.Http.Json;

namespace BooksWishlist.Infrastructure.Services;

public class HttpClientWrapper
{
    private readonly ILoggerService _log;

    public HttpClientWrapper(ILoggerService log) => _log = log;

    public async Task<T?> GetAsync<T>(string resourceQuery, CancellationToken cancellationToken = default)
        where T : class, new()
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

            if (httpRequestException.Message.ToLower().Contains("service unavailable)"))
            {
                //It is handled as a 404 since Google responds with an erroneous status for some requests.
                return null;
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
