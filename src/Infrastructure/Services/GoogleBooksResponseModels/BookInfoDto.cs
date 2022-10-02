namespace BooksWishlist.Infrastructure.Services.GoogleBooksResponseModels;

public class BookInfoDto
{
    public string? Id { get; set; }
    public string? SelfLink { get; set; }
    public string? Title { get; set; }
    public IEnumerable<string>? Authors { get; set; }
    public string? Publisher { get; set; }
    public string? PublishedDate { get; set; }
    public string? Description { get; set; }
    public string? ContentVersion { get; set; }
    public string? PreviewLink { get; set; }
    public string? InfoLink { get; set; }
    public string? Thumbnail { get; set; }
}
