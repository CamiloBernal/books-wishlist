using BooksWishlist.Application.Books.Entities;
using BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;

namespace BooksWishlist.Infrastructure.Services;

public interface IGoogleBooksService
{
    Task<GoogleBooksSearchResultDto?> Find(string q, string apiKey, BookSearchType? searchType,
        string? additionalTerm = "", int? page = 0, CancellationToken cancellationToken = default);

    Task<IEnumerable<Book?>?>? ValidateAndBindWishListBooksAsync(IEnumerable<string>? booksIds, string apiKey,
        CancellationToken cancellationToken = default);

    BookSearchType ParseSearchType(string? value);
}
