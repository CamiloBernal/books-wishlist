// ReSharper disable ClassNeverInstantiated.Global

namespace BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;

public class BookItem
{
    public string? Kind { get; set; }
    public string? Id { get; set; }
    public string? Etag { get; set; }
    public string? SelfLink { get; set; }
    public VolumeInfo? VolumeInfo { get; set; }
}
