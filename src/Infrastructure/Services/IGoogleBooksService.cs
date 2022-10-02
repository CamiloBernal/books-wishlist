using BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;

namespace BooksWishlist.Infrastructure.Services;

public interface IGoogleBooksService
{
    Task<GoogleBooksSearchResultDto?> Find(string q, string apiKey, BookSearchType? searchType, string? additionalTerm = "",  CancellationToken cancellationToken = default);
    BookSearchType ParseSearchType(string? value);
}
