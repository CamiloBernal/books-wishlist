using BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;

namespace BooksWishlist.Infrastructure.Services;

public class GoogleBooksService : IGoogleBooksService
{
    private readonly HttpClientWrapper<GoogleBooksSearchResults> _httpClientWrapper;

    public GoogleBooksService(ILoggerService log)
    {
        _httpClientWrapper = new HttpClientWrapper<GoogleBooksSearchResults>(log);
    }

    public async Task<GoogleBooksSearchResults?> Find(string q, string apiKey, BookSearchType? searchType,
        string? additionalTerm = "", CancellationToken cancellationToken = default)
    {
        if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));
        var urlToHandleRequest = BuildEndPointUrl(q, apiKey, searchType, additionalTerm);
        var results = await _httpClientWrapper.GetAsync(urlToHandleRequest, cancellationToken);
        return results;
    }

    public BookSearchType ParseSearchType(string value) => value.ToLower() switch
    {
        "title" => BookSearchType.InTitle,
        "author" => BookSearchType.InAuthor,
        "publisher" => BookSearchType.InPublisher,
        "subject" => BookSearchType.BySubject,
        "isbn" => BookSearchType.ByIsbn,
        "lccn" => BookSearchType.InLccn,
        "oclc" => BookSearchType.InOclc,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Search type not supported")
    };


    private static string BuildEndPointUrl(string q, string apiKey, BookSearchType? searchType,
        string? additionalTerm = "")
    {
        var searchTypeKeyWord = searchType switch
        {
            BookSearchType.InTitle => "+intitle",
            BookSearchType.InAuthor => "+inauthor",
            BookSearchType.InPublisher => "+inpublisher",
            BookSearchType.BySubject => "+subject",
            BookSearchType.ByIsbn => "+isbn",
            BookSearchType.InLccn => "+lccn",
            BookSearchType.InOclc => "+oclc",
            BookSearchType.FullSearch => "",
            _ => throw new ArgumentOutOfRangeException(nameof(searchType), searchType, "Search type not supported")
        };
        var query = string.IsNullOrEmpty(additionalTerm)
            ? q
            : $"{searchTypeKeyWord}:{additionalTerm}: {additionalTerm}";
        var urlToHandleRequest = $"{Constants.GoogleBooksServiceQueryBase}{query}&projection=lite&key={apiKey}";
        return urlToHandleRequest;
    }
}
