using BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;

namespace BooksWishlist.Infrastructure.Services;

public class GoogleBooksService : IGoogleBooksService
{
    private readonly HttpClientWrapper<GoogleBooksSearchResults> _httpClientWrapper;
    private readonly GoogleBooksServiceOptions _serviceOptions;
    private readonly ILoggerService _log;

    public GoogleBooksService(ILoggerService log, IOptions<GoogleBooksServiceOptions> serviceOptions)
    {
        _log = log;
        _serviceOptions = serviceOptions.Value;
        _httpClientWrapper = new HttpClientWrapper<GoogleBooksSearchResults>(log);
    }

    public async Task<GoogleBooksSearchResultDto?> Find(string q, string apiKey, BookSearchType? searchType,
        string? additionalTerm = "", int? page = 0, CancellationToken cancellationToken = default)
    {
        if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));
        var urlToHandleRequest = BuildEndPointUrl(q, apiKey, searchType, additionalTerm, page);
         _log.LogInformation($"Querying the GoogleBooks service through the url: {urlToHandleRequest}");
        var results = await _httpClientWrapper.GetAsync(urlToHandleRequest, cancellationToken);
        if (results == null) return null;
        var resultsDto =  (GoogleBooksSearchResultDto)results;
        resultsDto.CurrentPage = page ?? 1;
        resultsDto.TotalPages = resultsDto.TotalItems / _serviceOptions.MaxResults;
        return resultsDto;
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
        _ => throw new NonSupportedSearchTypeException()
    };

    private int? BuildStartIndexParameter(int? page = 0)
    {
        if (page == 0) return null;
        var maxResults = _serviceOptions.MaxResults;
        return (page - 1) * maxResults;
    }

    private string BuildEndPointUrl(string q, string apiKey, BookSearchType? searchType,
        string? additionalTerm = "", int? page = 0)
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
            _ => throw new NonSupportedSearchTypeException()
        };
        var startIndex = BuildStartIndexParameter(page);
        var query = string.IsNullOrEmpty(additionalTerm)
            ? q
            : $"{q}{searchTypeKeyWord}:{additionalTerm}";
        var urlToHandleRequest =
            $"{Constants.GoogleBooksServiceQueryBase}{query}&projection=lite&maxResults={_serviceOptions.MaxResults}&key={apiKey}";
        if (startIndex.HasValue) urlToHandleRequest += $"&startIndex={startIndex}";
        return urlToHandleRequest;
    }
}
