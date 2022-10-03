using BooksWishlist.Application.Books.Entities;
using BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;

namespace BooksWishlist.Infrastructure.Services;

public class GoogleBooksService : IGoogleBooksService
{
    private readonly HttpClientWrapper _httpClientWrapper;
    private readonly ILoggerService _log;
    private readonly GoogleBooksServiceOptions _serviceOptions;

    public GoogleBooksService(ILoggerService log, IOptions<GoogleBooksServiceOptions> serviceOptions)
    {
        _log = log;
        _serviceOptions = serviceOptions.Value;
        _httpClientWrapper = new HttpClientWrapper(log);
    }

    public async Task<GoogleBooksSearchResultDto?> Find(string q, string apiKey, BookSearchType? searchType,
        string? additionalTerm = "", int? page = 0, CancellationToken cancellationToken = default)
    {
        if (apiKey == null)
        {
            throw new ArgumentNullException(nameof(apiKey));
        }

        var urlToHandleRequest = BuildEndPointUrl(q, apiKey, searchType, additionalTerm, page);
        _log.LogInformation($"Querying the GoogleBooks service through the url: {urlToHandleRequest}");
        var results = await _httpClientWrapper.GetAsync<GoogleBooksSearchResults>(urlToHandleRequest, cancellationToken)
            .ConfigureAwait(false);
        if (results == null)
        {
            return null;
        }

        var resultsDto = (GoogleBooksSearchResultDto)results;
        resultsDto.CurrentPage = page ?? 1;
        resultsDto.TotalPages = resultsDto.TotalItems / _serviceOptions.MaxResults;
        return resultsDto;
    }

    public async Task<IEnumerable<Book?>?>? ValidateAndBindWishListBooksAsync(IEnumerable<string>? booksIds,
        string apiKey, CancellationToken cancellationToken)
    {
        if (booksIds is null)
        {
            return null;
        }

        var validationTask = booksIds.Select(i => ValidateAndBindBook(i, apiKey, cancellationToken)).ToList();
        var foundBooks = await Task.WhenAll(validationTask);
        return foundBooks;
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


    public async Task<Book?> ValidateAndBindBook(string bookId, string apiKey,
        CancellationToken cancellationToken = default)
    {
        var query = $"{Constants.GoogleBooksServiceVolumesBase}{bookId}?key={apiKey}";
        var bookItem = await _httpClientWrapper.GetAsync<BookItem>(query, cancellationToken);
        if (bookItem is not null)
        {
            return BindBook(bookItem);
        }

        throw new InvalidBookReferenceException($"The book with id {bookId} was not found on the google books server.");
    }

    private static Book BindBook(BookItem bookItem) => new()
    {
        Authors = bookItem.VolumeInfo?.Authors,
        Description = bookItem.VolumeInfo?.Description,
        Publisher = bookItem.VolumeInfo?.Publisher,
        Title = bookItem.VolumeInfo?.Title,
        BookId = bookItem.Id
    };


    private int? BuildStartIndexParameter(int? page = 0)
    {
        if (page == 0)
        {
            return null;
        }

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
        if (startIndex.HasValue)
        {
            urlToHandleRequest += $"&startIndex={startIndex}";
        }

        return urlToHandleRequest;
    }
}
