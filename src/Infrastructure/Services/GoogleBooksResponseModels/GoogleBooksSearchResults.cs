// ReSharper disable ClassNeverInstantiated.Global
namespace BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;

public class GoogleBooksSearchResults
{
    public string? Kind { get; set; }
    public int? TotalItems { get; set; } = 0;
    public IEnumerable<BookItem>? Items { get; set; }
}
